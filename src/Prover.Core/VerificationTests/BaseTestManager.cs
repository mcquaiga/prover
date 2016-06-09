using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using NLog;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Collections;
using Prover.Core.Communication;
using Prover.Core.Events;
using Prover.Core.EVCTypes;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;
using Prover.Core.Storage;
using LogManager = NLog.LogManager;

namespace Prover.Core.VerificationTests
{
    public class TestManager
    {
        private const int VolumeTestnumber = 0;

        protected static Logger Log = LogManager.GetCurrentClassLogger();
        protected readonly IUnityContainer Container;
        private bool _isBusy;

        private bool _isLiveReading;
        private bool _stopLiveReading;

        protected TestManager(IUnityContainer container, Instrument instrument, EvcCommunicationClient commClient)
        {
            Container = container;
            Instrument = instrument;
            CommunicationClient = commClient;
        }

        public Instrument Instrument { get; }
        public EvcCommunicationClient CommunicationClient { get; }
        public virtual BaseVolumeVerificationManager VolumeTestManager { get; set; }

        public async Task RunTest(int level)
        {
            await WaitForReadingsToStablize(level);

            await DownloadVerificationTestItems(level);

            if (Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == level)?.VolumeTest != null)
                await VolumeTestManager.StartVolumeTest();
        }

        private async Task WaitForReadingsToStablize(int level)
        {
            var liveReadItems = new Dictionary<int, ReadingStabilizer>();
            if (Instrument.CompositionType == CorrectorType.PTZ)
            {
                liveReadItems.Add(8, new ReadingStabilizer(GetGaugePressure(Instrument, level)));
                liveReadItems.Add(26, new ReadingStabilizer(GetGaugeTemp(level)));
            }

            if (Instrument.CompositionType == CorrectorType.T)
            {
                liveReadItems.Add(26, new ReadingStabilizer(GetGaugeTemp(level)));
            }

            if (Instrument.CompositionType == CorrectorType.P)
            {
                liveReadItems.Add(8, new ReadingStabilizer(GetGaugePressure(Instrument, level)));
            }

            do
            {
                foreach (var item in liveReadItems)
                {
                    var liveValue = await CommunicationClient.LiveReadItemValue(item.Key);
                    item.Value.ValueQueue.Enqueue(liveValue.NumericValue);
                    Container.Resolve<IEventAggregator>()
                        .PublishOnBackgroundThread(new LiveReadEvent(item.Key, liveValue.NumericValue));
                }
            } while (liveReadItems.Any(x => !x.Value.IsStable()));

            await CommunicationClient.Disconnect();
        }

        public async Task DownloadVerificationTestItems(int level)
        {
            if (Instrument.CompositionType == CorrectorType.PTZ)
            {
                await DownloadTemperatureTestItems(level, false);
                await DownloadPressureTestItems(level);
            }

            if (Instrument.CompositionType == CorrectorType.T)
            {
                await DownloadTemperatureTestItems(level);
            }

            if (Instrument.CompositionType == CorrectorType.P)
            {
                await DownloadPressureTestItems(level);
            }
        }

        public async Task DownloadTemperatureTestItems(int levelNumber, bool disconnectAfter = true)
        {
            var test = Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == levelNumber).TemperatureTest;

            if (test != null)
                test.Items = await CommunicationClient.GetItemValues(CommunicationClient.ItemDetails.TemperatureItems());
        }

        public async Task DownloadPressureTestItems(int level, bool disconnectAfter = true)
        {
            if (_isLiveReading) await StopLiveRead();

            var test = Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == level).PressureTest;
            if (test != null)
                test.Items = await CommunicationClient.GetItemValues(CommunicationClient.ItemDetails.PressureItems());

            _isBusy = false;
        }

        public async Task StartLiveRead()
        {
            var liveReadItems = new List<int>();
            if (Instrument.CompositionType == CorrectorType.PTZ)
            {
                liveReadItems.Add(8);
                liveReadItems.Add(26);
            }

            if (Instrument.CompositionType == CorrectorType.T)
            {
                liveReadItems.Add(26);
            }

            if (Instrument.CompositionType == CorrectorType.P)
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
                Log.Debug("Starting live read...");
                await CommunicationClient.Connect();
                _stopLiveReading = false;
                _isLiveReading = true;
                do
                {
                    foreach (var i in itemNumbers)
                    {
                        var liveValue = await CommunicationClient.LiveReadItemValue(i);
                        Container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new LiveReadEvent(i, liveValue.NumericValue));
                    }
                } while (!_stopLiveReading);

                await CommunicationClient.Disconnect();
                _isLiveReading = false;
                _isBusy = false;
                Log.Debug("Finished live read!");
            }
        }

        public async Task StopLiveRead()
        {
            if (_isLiveReading)
            {
                _stopLiveReading = true;
                await CommunicationClient.Disconnect();
            }
        }

        public async Task SaveAsync()
        {
            using (var store = new InstrumentStore(Container))
            {
                await store.UpsertAsync(Instrument);
            }
        }

        protected static void CreateVerificationTests(Instrument instrument, IDriveType driveType)
        {
            for (var i = 0; i < 3; i++)
            {
                var verificationTest = new VerificationTest(i, instrument);

                if (instrument.CompositionType == CorrectorType.P)
                {
                    verificationTest.PressureTest = new PressureTest(verificationTest, GetGaugePressure(instrument, i));
                }

                if (instrument.CompositionType == CorrectorType.T)
                {
                    verificationTest.TemperatureTest = new TemperatureTest(verificationTest, GetGaugeTemp(i));
                }

                if (instrument.CompositionType == CorrectorType.PTZ)
                {
                    verificationTest.PressureTest = new PressureTest(verificationTest, GetGaugePressure(instrument, i));
                    verificationTest.TemperatureTest = new TemperatureTest(verificationTest, GetGaugeTemp(i));
                    verificationTest.SuperFactorTest = new SuperFactorTest(verificationTest);
                }

                if (i == VolumeTestnumber)
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
            var value =
                SettingsManager.SettingsInstance.PressureGaugeDefaults.FirstOrDefault(p => p.Level == testNumber).Value;

            if (value > 1)
                value = value/100;

            var evcPressureRange = instrument.Items.GetItem(ItemCodes.Pressure.Range).NumericValue;
            return value*evcPressureRange;
        }
    }
}