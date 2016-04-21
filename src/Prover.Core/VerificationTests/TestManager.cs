using Caliburn.Micro;
using Microsoft.Practices.Unity;
using NLog;
using Prover.Core.Events;
using Prover.Core.Models;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.Core.Communication;
using Prover.SerialProtocol;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Prover.Core.VerificationTests
{
    public class RotaryTestManager : ITestManager
    {
        private Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private readonly IUnityContainer _container;
        private bool _isLiveReading = false;
        private bool _stopLiveReading;
        private bool _isBusy = false;
        
        public Instrument Instrument { get; private set; }
        public InstrumentCommunicator InstrumentCommunicator { get; private set; }
        public TachometerCommunicator TachometerCommunicator { get; private set; }
        private RotaryVolumeVerification VolumeTest { get; }

        public static async Task<RotaryTestManager> Create(IUnityContainer container, InstrumentType instrumentType, ICommPort instrumentPort, string tachometerPortName)
        {
            var instrumentComm = new InstrumentCommunicator(container.Resolve<IEventAggregator>(), instrumentPort, instrumentType);
            var tachComm = new TachometerCommunicator(tachometerPortName);

            var items = new InstrumentItems(instrumentType);
            var itemValues = await instrumentComm.DownloadItemsAsync(items.Items.ToList());
            var instrument = new Instrument(instrumentType, items, itemValues);

            var manager = new RotaryTestManager(container, instrument, instrumentComm, tachComm);
            container.RegisterInstance(manager);

            return manager;
        }

        private RotaryTestManager(IUnityContainer container, Instrument instrument, InstrumentCommunicator instrumentCommunicator, TachometerCommunicator tachCommunicator) 
        {
            _container = container;
            Instrument = instrument;
            InstrumentCommunicator = instrumentCommunicator;
            TachometerCommunicator = tachCommunicator;
            
            instrument.VerificationTests.Add(new VerificationTest(0, instrument));
            instrument.VerificationTests.Add(new VerificationTest(1, instrument));

            var volumeVerification = new VerificationTest(2, instrument, true);
            instrument.VerificationTests.Add(volumeVerification);
            VolumeTest = new RotaryVolumeVerification(_container.Resolve<IEventAggregator>(), volumeVerification, InstrumentCommunicator, tachCommunicator);
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

        public async Task StartLiveRead(int itemNumber)
        {
            if (!_isBusy)
            {
                _log.Debug("Starting live read...");
                await InstrumentCommunicator.Connect();
                _stopLiveReading = false;
                _isLiveReading = true;
                do
                {
                    var liveValue = await InstrumentCommunicator.LiveReadItem(itemNumber);
                    _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new LiveReadEvent(liveValue));
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

        public async Task StartVolumeTest()
        {
            await VolumeTest.BeginVerificationTest();
        }

        public void StopVolumeTest()
        {
            VolumeTest.StopRunningTest();
        }
    }
}
