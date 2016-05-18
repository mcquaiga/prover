﻿using Caliburn.Micro;
using Microsoft.Practices.Unity;
using NLog;
using Prover.Core.Communication;
using Prover.Core.EVCTypes;
using Prover.Core.Events;
using Prover.Core.Models;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.SerialProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Core.Collections;
using Prover.Core.Extensions;
using Prover.Core.Settings;

namespace Prover.Core.VerificationTests
{
    public class TestManager
    {
        private const int VOLUME_TESTNUMBER = 0;

        protected static Logger _log = NLog.LogManager.GetCurrentClassLogger();
        protected readonly IUnityContainer _container;
        
        private bool _isLiveReading;
        private bool _isBusy;
        private bool _stopLiveReading;

        public Instrument Instrument { get; private set; }
        public InstrumentCommunicator InstrumentCommunicator { get; private set; }
        public virtual BaseVolumeVerificationManager VolumeTestManager { get; set; }

        protected TestManager(IUnityContainer container, Instrument instrument, InstrumentCommunicator instrumentComm)
        {
            _container = container;
            Instrument = instrument;
            InstrumentCommunicator = instrumentComm;
        }

        public async Task RunTest(int level)
        {
            await WaitForReadingsToStablize(level);

            await DownloadVerificationTestItems(level);

            if (Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == level)?.VolumeTest != null)
                await VolumeTestManager.StartVolumeTest();

            await SaveAsync();
        }

        private async Task WaitForReadingsToStablize(int level)
        {
            var liveReadItems = new Dictionary<int, ReadingStabilizer>();
            if (Instrument.CorrectorType == CorrectorType.PressureTemperature)
            {
                liveReadItems.Add(8, new ReadingStabilizer(GetGaugePressure(Instrument, level)));
                liveReadItems.Add(26, new ReadingStabilizer(GetGaugeTemp(level)));
            }

            if (Instrument.CorrectorType == CorrectorType.TemperatureOnly)
            {
                liveReadItems.Add(26, new ReadingStabilizer(GetGaugeTemp(level)));
            }

            if (Instrument.CorrectorType == CorrectorType.PressureOnly)
            {
                liveReadItems.Add(8, new ReadingStabilizer(GetGaugePressure(Instrument, level)));
            }

            do
            {
                foreach (var item in liveReadItems)
                {
                    var liveValue = await InstrumentCommunicator.LiveReadItem(item.Key);
                    item.Value.ValueQueue.Enqueue(liveValue);
                    _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new LiveReadEvent(item.Key, liveValue));
                }
            } while (liveReadItems.Any(x => !x.Value.IsStable()));

            await InstrumentCommunicator.Disconnect();
            
        }

        public async Task DownloadVerificationTestItems(int level)
        {
            if (Instrument.CorrectorType == CorrectorType.PressureTemperature)
            {
                await DownloadTemperatureTestItems(level, false);
                await DownloadPressureTestItems(level);
            }

            if (Instrument.CorrectorType == CorrectorType.TemperatureOnly)
            {
                await DownloadTemperatureTestItems(level);
            }

            if (Instrument.CorrectorType == CorrectorType.PressureOnly)
            {
                await DownloadPressureTestItems(level);
            }
        }

        public async Task DownloadTemperatureTestItems(int levelNumber, bool disconnectAfter = true)
        {
            if (_isLiveReading) await StopLiveRead();

            var test = Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == levelNumber).TemperatureTest;
            var itemsToDownload = Instrument.Items.Items.Where(i => i.IsTemperatureTest == true).ToList();
            if (test != null)
                test.ItemValues = await InstrumentCommunicator.DownloadItemsAsync(itemsToDownload, disconnectAfter);

            _isBusy = false;
        }

        public async Task DownloadPressureTestItems(int level, bool disconnectAfter = true)
        {
            if (_isLiveReading) await StopLiveRead();

            var test = Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == level).PressureTest;
            var itemsToDownload = Instrument.Items.Items.Where(i => i.IsPressureTest == true).ToList();
            if (test != null)
                test.ItemValues = await InstrumentCommunicator.DownloadItemsAsync(itemsToDownload, disconnectAfter);

            _isBusy = false;
        }

        public async Task StartLiveRead()
        {
            var liveReadItems = new List<int>();
            if (Instrument.CorrectorType == CorrectorType.PressureTemperature)
            {
                liveReadItems.Add(8);
                liveReadItems.Add(26);
            }

            if (Instrument.CorrectorType == CorrectorType.TemperatureOnly)
            {
                liveReadItems.Add(26);
            }

            if (Instrument.CorrectorType == CorrectorType.PressureOnly)
            {
                liveReadItems.Add(8);
            }

            await StartLiveRead(liveReadItems);
        }

        public async Task StartLiveRead(IEnumerable<int> itemNumbers)
        {
            if (!_isBusy)
            {
                var itemQueues = new Dictionary<int, FixedSizedQueue<decimal>>();
                _log.Debug("Starting live read...");
                await InstrumentCommunicator.Connect();
                _stopLiveReading = false;
                _isLiveReading = true;
                do
                {
                    foreach (var i in itemNumbers)
                    {
                         var liveValue = await InstrumentCommunicator.LiveReadItem(i);
                        _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new LiveReadEvent(i, liveValue));
                    }
                } while (!_stopLiveReading);

                await InstrumentCommunicator.Disconnect();
                _isLiveReading = false;
                _isBusy = false;
                _log.Debug("Finished live read!");
            }
        }

        public async Task StopLiveRead()
        {
            if (_isLiveReading)
            {
                _stopLiveReading = true;
                await InstrumentCommunicator.Disconnect();
            }
        }

        public async Task SaveAsync()
        {
            using (var store = new InstrumentStore(_container))
            {
                await store.UpsertAsync(Instrument);
            }
        }

        protected static void CreateVerificationTests(Instrument instrument, IDriveType driveType)
        {
            for (int i = 0; i < 3; i++)
            {
                var verificationTest = new VerificationTest(i, instrument);

                if (instrument.CorrectorType == CorrectorType.PressureOnly)
                {
                    verificationTest.PressureTest = new PressureTest(verificationTest, GetGaugePressure(instrument, i));
                }

                if (instrument.CorrectorType == CorrectorType.TemperatureOnly)
                {
                    verificationTest.TemperatureTest = new TemperatureTest(verificationTest, GetGaugeTemp(i));
                }

                if (instrument.CorrectorType == CorrectorType.PressureTemperature)
                {
                    verificationTest.PressureTest = new PressureTest(verificationTest, GetGaugePressure(instrument, i));
                    verificationTest.TemperatureTest = new TemperatureTest(verificationTest, GetGaugeTemp(i));
                    verificationTest.SuperFactorTest = new SuperFactorTest(verificationTest);
                }

                if (i == VOLUME_TESTNUMBER)
                    verificationTest.VolumeTest = new VolumeTest(verificationTest, driveType);

                instrument.VerificationTests.Add(verificationTest);
            }
        }
    
        private static decimal GetGaugeTemp(int testNumber)
        {
            return
                SettingsManager.SettingsInstance.TemperatureGaugeDefaults.FirstOrDefault(t => t.Level == testNumber)
                    .Value;
        }

        private static decimal GetGaugePressure(Instrument instrument, int testNumber)
        {
            var value = SettingsManager.SettingsInstance.PressureGaugeDefaults.FirstOrDefault(p => p.Level == testNumber).Value;

            if (value > 1)
                value = value/100;

            var evcPressureRange = instrument.EvcPressureRange();
            if (evcPressureRange != null)
                return value * evcPressureRange.Value;
            else
            {
                return 0m;
            }
        }
    }
}
