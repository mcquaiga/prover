using System;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.ExternalIntegrations.Validators;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.Core.VerificationTests.VolumeVerification;

namespace Prover.Core.VerificationTests
{
    public interface IQaRunTestManager : IDisposable
    {
        Instrument Instrument { get; }
        Task InitializeTest(InstrumentType instrumentType);
        Task RunTest(int level);
        Task DownloadVerificationTestItems(int level);
        Task DownloadTemperatureTestItems(int levelNumber);
        Task DownloadPressureTestItems(int level);
        Task SaveAsync();
        Task RunVerifier();
    }

    public class QaRunTestManager : IQaRunTestManager
    {
        private const int VolumeTestNumber = 0;
        protected static Logger Log = LogManager.GetCurrentClassLogger();
        private readonly EvcCommunicationClient _communicationClient;
        private readonly IInstrumentStore<Instrument> _instrumentStore;
        private readonly IReadingStabilizer _readingStabilizer;
        private readonly IValidator _validator;

        public QaRunTestManager(
            IInstrumentStore<Instrument> instrumentStore,
            EvcCommunicationClient commClient,
            IReadingStabilizer readingStabilizer,
            VolumeTestManagerBase volumeTestManager,
            IValidator validator
        )
        {
            VolumeTestManager = volumeTestManager;
            _instrumentStore = instrumentStore;
            _communicationClient = commClient;
            _readingStabilizer = readingStabilizer;
            _validator = validator;
        }

        public VolumeTestManagerBase VolumeTestManager { get; set; }

        public void Dispose()
        {
            _communicationClient.Dispose();
        }

        public Instrument Instrument { get; private set; }

        public async Task InitializeTest(InstrumentType instrumentType)
        {
            _communicationClient.Initialize(instrumentType);
            await _communicationClient.Connect();
            var items = await _communicationClient.GetItemValues(_communicationClient.ItemDetails.GetAllItemNumbers());
            await _communicationClient.Disconnect();

            Instrument = new Instrument(instrumentType, items);

            await SaveAsync();
            await RunVerifier();
        }

        public async Task RunTest(int level)
        {
            if (Instrument == null) throw new NullReferenceException("Call InitializeTest before runnning a test");

            await _readingStabilizer.WaitForReadingsToStabilizeAsync(_communicationClient, Instrument, level);
            await DownloadVerificationTestItems(level);

            if (Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == level)?.VolumeTest != null)
                await VolumeTestManager.RunTest(_communicationClient, Instrument.VolumeTest, null);

            await SaveAsync();
        }

        public async Task DownloadVerificationTestItems(int level)
        {
            if (!_communicationClient.IsConnected)
                await _communicationClient.Connect();

            if (Instrument.CompositionType == CorrectorType.PTZ)
            {
                await DownloadTemperatureTestItems(level);
                await DownloadPressureTestItems(level);
            }

            if (Instrument.CompositionType == CorrectorType.T)
                await DownloadTemperatureTestItems(level);

            if (Instrument.CompositionType == CorrectorType.P)
                await DownloadPressureTestItems(level);

            await _communicationClient.Disconnect();
        }

        public async Task DownloadTemperatureTestItems(int levelNumber)
        {
            var test = Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == levelNumber).TemperatureTest;

            if (test != null)
                test.Items =
                    await _communicationClient.GetItemValues(_communicationClient.ItemDetails.TemperatureItems());
        }

        public async Task DownloadPressureTestItems(int level)
        {
            var test = Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == level).PressureTest;
            if (test != null)
                test.Items = await _communicationClient.GetItemValues(_communicationClient.ItemDetails.PressureItems());
        }

        public async Task SaveAsync()
        {
            try
            {
                await _instrumentStore.UpsertAsync(Instrument);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public async Task RunVerifier()
        {
            if (_validator != null)
                await _validator.Validate(_communicationClient, Instrument);
        }
    }
}