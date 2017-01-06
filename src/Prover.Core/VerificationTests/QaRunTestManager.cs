using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using NLog;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.ExternalIntegrations.Validators;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.Core.VerificationTests.VolumeVerification;

namespace Prover.Core.VerificationTests
{
    public interface IQaRunTestManager : IDisposable
    {
        Instrument Instrument { get; }
        Task InitializeTest(InstrumentType instrumentType, Client client = null);
        Task RunTest(int level);
        Task DownloadVerificationTestItems(int level);
        Task DownloadTemperatureTestItems(int levelNumber);
        Task DownloadPressureTestItems(int level);
        Task SaveAsync();
        Task RunVerifiers();

        IObservable<string> TestStatus { get; }
    }

    public class QaRunTestManager : IQaRunTestManager
    {
        private const int VolumeTestNumber = 0;
        protected static Logger Log = LogManager.GetCurrentClassLogger();
        private readonly EvcCommunicationClient _communicationClient;
        private readonly IProverStore<Instrument> _instrumentStore;
        private readonly IReadingStabilizer _readingStabilizer;
        private readonly IEnumerable<IValidator> _validators;
        private readonly IEvcItemReset _itemResetter;
        private readonly Subject<string> _testStatus = new Subject<string>();
        private Client _client;

        public QaRunTestManager(
            IProverStore<Instrument> instrumentStore,
            EvcCommunicationClient commClient,
            IReadingStabilizer readingStabilizer,
            VolumeTestManagerBase volumeTestManager,
            IEnumerable<IValidator> validators = null,
            IEvcItemReset itemResetter = null
        )
        {
            VolumeTestManager = volumeTestManager;
            _instrumentStore = instrumentStore;
            _communicationClient = commClient;
            _readingStabilizer = readingStabilizer;
            _validators = validators;
            _itemResetter = itemResetter;
        }

        public IObservable<string> TestStatus => _testStatus.AsObservable();

        public VolumeTestManagerBase VolumeTestManager { get; set; }

        public void Dispose()
        {
            _communicationClient.Dispose();
        }

        public Instrument Instrument { get; private set; }

        public async Task InitializeTest(InstrumentType instrumentType, Client client = null)
        {
            _client = client;

            _communicationClient.Initialize(instrumentType);

            _testStatus.OnNext($"Connecting to {instrumentType.Name}...");
            await _communicationClient.Connect();

            _testStatus.OnNext("Downloading items...");
            var items = await _communicationClient.GetItemValues(_communicationClient.ItemDetails.GetAllItemNumbers());

            _testStatus.OnNext($"Disconnecting from {instrumentType.Name}...");
            await _communicationClient.Disconnect();

            Instrument = new Instrument(instrumentType, items, client);

            await SaveAsync();
            await RunVerifiers();
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

            _testStatus.OnNext($"Resetting items...");
            var resetItems = _client.Items.FirstOrDefault(x => x.ItemFileType == ItemType.PostTest && x.InstrumentType == Instrument.InstrumentType)?.Items.ToList();
            if (_itemResetter != null && resetItems != null && resetItems.Any())
            {
                await _itemResetter?.PostReset(_communicationClient, resetItems);
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

        public async Task RunVerifiers()
        {
            if (_validators != null && _validators.Any())
            {
                foreach (var validator in _validators)
                {
                    _testStatus.OnNext($"Verifying items...");
                    await validator.Validate(_communicationClient, Instrument);
                }
            }       
        }
    }
}