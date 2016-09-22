using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using NLog;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Events;
using Prover.Core.EVCTypes;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;
using Prover.Core.Storage;
using Prover.Core.VerificationTests.VolumeTest;
using LogManager = NLog.LogManager;

namespace Prover.Core.VerificationTests
{
    public class TestManager : IDisposable
    {
        private const int VolumeTestNumber = 0;
        protected static Logger Log = LogManager.GetCurrentClassLogger();
        protected readonly IUnityContainer Container;

        protected TestManager(IUnityContainer container, Instrument instrument, EvcCommunicationClient commClient,
            IVerifier verifier)
        {
            Container = container;
            Instrument = instrument;
            CommunicationClient = commClient;
            Verifier = verifier;
        }

        public IVerifier Verifier { get; private set; }
        public Instrument Instrument { get; }
        public EvcCommunicationClient CommunicationClient { get; }
        public virtual VolumeTestManager VolumeTestManager { get; set; }

        public void Dispose()
        {
            CommunicationClient.Dispose();
        }

        public async Task RunTest(int level)
        {
            await WaitForReadingsToStablize(level);

            await DownloadVerificationTestItems(level);

            if (Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == level)?.VolumeTest != null)
                await VolumeTestManager.PreTest();
        }

        private async Task WaitForReadingsToStablize(int level)
        {
            var liveReadItems = GetLiveReadItemNumbers(level);

            await LiveReadItems(liveReadItems);
        }

        private Dictionary<int, ReadingStabilizer> GetLiveReadItemNumbers(int level)
        {
            var liveReadItems = new Dictionary<int, ReadingStabilizer>();
            if (Instrument.CompositionType == CorrectorType.PTZ)
            {
                liveReadItems.Add(8, new ReadingStabilizer(GetGaugePressure(Instrument, level)));
                liveReadItems.Add(26, new ReadingStabilizer(GetGaugeTemp(level)));
            }

            if (Instrument.CompositionType == CorrectorType.T)
                liveReadItems.Add(26, new ReadingStabilizer(GetGaugeTemp(level)));

            if (Instrument.CompositionType == CorrectorType.P)
                liveReadItems.Add(8, new ReadingStabilizer(GetGaugePressure(Instrument, level)));
            return liveReadItems;
        }

        private async Task LiveReadItems(Dictionary<int, ReadingStabilizer> liveReadItems)
        {
            await CommunicationClient.Connect();

            do
            {
                foreach (var item in liveReadItems)
                {
                    var liveValue = await CommunicationClient.LiveReadItemValue(item.Key);
                    item.Value.ValueQueue.Enqueue(liveValue.NumericValue);
                    Container.Resolve<IEventAggregator>()
                        .PublishOnBackgroundThread(new LiveReadEvent(item.Key, liveValue.NumericValue));
                }
            } while (liveReadItems.Any(x => !x.Value.IsStable));

            await CommunicationClient.Disconnect();
        }

        public async Task DownloadVerificationTestItems(int level)
        {
            if (!CommunicationClient.IsConnected)
                await CommunicationClient.Connect();

            if (Instrument.CompositionType == CorrectorType.PTZ)
            {
                await DownloadTemperatureTestItems(level);
                await DownloadPressureTestItems(level);
            }

            if (Instrument.CompositionType == CorrectorType.T)
                await DownloadTemperatureTestItems(level);

            if (Instrument.CompositionType == CorrectorType.P)
                await DownloadPressureTestItems(level);

            await CommunicationClient.Disconnect();
        }

        public async Task DownloadTemperatureTestItems(int levelNumber)
        {
            var test = Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == levelNumber).TemperatureTest;

            if (test != null)
                test.Items = await CommunicationClient.GetItemValues(CommunicationClient.ItemDetails.TemperatureItems());
        }

        public async Task DownloadPressureTestItems(int level)
        {
            var test = Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == level).PressureTest;
            if (test != null)
                test.Items = await CommunicationClient.GetItemValues(CommunicationClient.ItemDetails.PressureItems());
        }

        public async Task SaveAsync()
        {
            try
            {
                using (var store = new InstrumentStore(Container))
                {
                    await store.UpsertAsync(Instrument);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        protected static void CreateVerificationTests(Instrument instrument, IDriveType driveType)
        {
            for (var i = 0; i < 3; i++)
            {
                var verificationTest = new VerificationTest(i, instrument);

                if (instrument.CompositionType == CorrectorType.P)
                    verificationTest.PressureTest = new PressureTest(verificationTest, GetGaugePressure(instrument, i));

                if (instrument.CompositionType == CorrectorType.T)
                    verificationTest.TemperatureTest = new TemperatureTest(verificationTest, GetGaugeTemp(i));

                if (instrument.CompositionType == CorrectorType.PTZ)
                {
                    verificationTest.PressureTest = new PressureTest(verificationTest, GetGaugePressure(instrument, i));
                    verificationTest.TemperatureTest = new TemperatureTest(verificationTest, GetGaugeTemp(i));
                    verificationTest.SuperFactorTest = new SuperFactorTest(verificationTest);
                }

                if (i == VolumeTestNumber)
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

        public async Task RunVerifier()
        {
            Verifier = Container.Resolve<IVerifier>(new ParameterOverride("commClient", CommunicationClient),
                new ParameterOverride("instrument", Instrument));
            var success = await Verifier.Verify();
        }
    }
}