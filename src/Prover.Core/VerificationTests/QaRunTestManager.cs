using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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

        IObservable<string> TestStatus { get; }
    }

    public class QaRunTestManager : IQaRunTestManager
    {
        private const int VolumeTestNumber = 0;
        protected static Logger Log = LogManager.GetCurrentClassLogger();
        private readonly EvcCommunicationClient _communicationClient;
        private readonly IProverStore<Instrument> _instrumentStore;
        private readonly IReadingStabilizer _readingStabilizer;
        private readonly IValidator _validator;
        private readonly IEvcItemReset _itemResetter;
        private readonly Subject<string> _testStatus = new Subject<string>();

        public QaRunTestManager(
            IProverStore<Instrument> instrumentStore,
            EvcCommunicationClient commClient,
            IReadingStabilizer readingStabilizer,
            VolumeTestManagerBase volumeTestManager,
            IValidator validator,
            IEvcItemReset itemResetter = null
        )
        {
            VolumeTestManager = volumeTestManager;
            _instrumentStore = instrumentStore;
            _communicationClient = commClient;
            _readingStabilizer = readingStabilizer;
            _validator = validator;
            _itemResetter = itemResetter;
        }

        public IObservable<string> TestStatus => _testStatus.AsObservable();

        public VolumeTestManagerBase VolumeTestManager { get; set; }

        public void Dispose()
        {
            _communicationClient.Dispose();
        }

        public Instrument Instrument { get; private set; }

        public async Task InitializeTest(InstrumentType instrumentType)
        {
            _communicationClient.Initialize(instrumentType);

            _testStatus.OnNext($"Connecting to {instrumentType.Name}...");
            await _communicationClient.Connect();

            _testStatus.OnNext("Downloading items...");
            var items = await _communicationClient.GetItemValues(_communicationClient.ItemDetails.GetAllItemNumbers());

            _testStatus.OnNext($"Disconnecting from {instrumentType.Name}...");
            await _communicationClient.Disconnect();

            Instrument = new Instrument(instrumentType, items);

            await SaveAsync();
            await RunVerifier();
        }

        public async Task RunTest(int level)
        {
            if (Instrument == null) throw new NullReferenceException("Call InitializeTest before runnning a test");

            _testStatus.OnNext($"Waiting for live readings to stabilize...");
            await _readingStabilizer.WaitForReadingsToStabilizeAsync(_communicationClient, Instrument, level);

            _testStatus.OnNext($"Download items...");
            await DownloadVerificationTestItems(level);

            if (Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == level)?.VolumeTest != null)
            {
                _testStatus.OnNext($"Running volume test...");
                await VolumeTestManager.RunTest(_communicationClient, Instrument.VolumeTest, _itemResetter);
            }
            _testStatus.OnNext($"Saving test...");
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
            var firstOrDefault = Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == levelNumber);
            var test = firstOrDefault?.TemperatureTest;

            if (test != null)
                test.Items =
                    await _communicationClient.GetItemValues(_communicationClient.ItemDetails.TemperatureItems());
        }

        public async Task DownloadPressureTestItems(int level)
        {
            var firstOrDefault = Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == level);
            var test = firstOrDefault?.PressureTest;
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
            {
                _testStatus.OnNext($"Verifying items...");
                await _validator.Validate(_communicationClient, Instrument);  
            }       
        }
    }
}