using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.ExternalIntegrations.Validators;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.Core.VerificationTests.TestActions;
using Prover.Core.VerificationTests.VolumeVerification;

namespace Prover.Core.VerificationTests
{
    public interface IQaRunTestManager : IDisposable
    {
        Instrument Instrument { get; }

        IObservable<string> TestStatus { get; }
        Task InitializeTest(InstrumentType instrumentType, CancellationToken ct = new CancellationToken(), Client client = null);
        Task RunTest(int level, CancellationToken ct = new CancellationToken());
        Task SaveAsync();
        Task RunVerifiers();
    }

    public class QaRunTestManager : IQaRunTestManager
    {
        protected static Logger Log = LogManager.GetCurrentClassLogger();
        private readonly EvcCommunicationClient _communicationClient;
        private readonly IProverStore<Instrument> _instrumentStore;
        private readonly IReadingStabilizer _readingStabilizer;
        private readonly Subject<string> _testStatus = new Subject<string>();

        private readonly IEnumerable<PreTestValidationBase> _validators;
        private readonly IEnumerable<PostTestResetBase> _postTestCommands;

        public QaRunTestManager(
            IProverStore<Instrument> instrumentStore,
            EvcCommunicationClient commClient,
            IReadingStabilizer readingStabilizer,
            VolumeTestManagerBase volumeTestManager,
            IEnumerable<PreTestValidationBase> validators = null,
            IEnumerable<PostTestResetBase> postTestCommands = null)
        {
            VolumeTestManager = volumeTestManager;
            _instrumentStore = instrumentStore;
            _communicationClient = commClient;
            _readingStabilizer = readingStabilizer;
            _validators = validators;
            _postTestCommands = postTestCommands;
        }

        public VolumeTestManagerBase VolumeTestManager { get; set; }

        public IObservable<string> TestStatus => _testStatus.AsObservable();

        public Instrument Instrument { get; private set; }

        public async Task InitializeTest(InstrumentType instrumentType, CancellationToken ct, Client client = null)
        {
            _communicationClient.Initialize(instrumentType);

            _testStatus.OnNext($"Connecting to {instrumentType.Name}...");
            await _communicationClient.Connect(ct);

            _testStatus.OnNext("Downloading items...");
            var items = await _communicationClient.GetItemValues(_communicationClient.ItemDetails.GetAllItemNumbers());

            _testStatus.OnNext($"Disconnecting from {instrumentType.Name}...");
            await _communicationClient.Disconnect();

            Instrument = new Instrument(instrumentType, items, client);

            await RunVerifiers();
        }

        public async Task RunTest(int level, CancellationToken ct)
        {
            try
            {
                if (Instrument == null) throw new NullReferenceException("Call InitializeTest before runnning a test");

                _testStatus.OnNext($"Stabilizing live readings...");
                await _readingStabilizer.WaitForReadingsToStabilizeAsync(_communicationClient, Instrument, level, ct);

                _testStatus.OnNext($"Downloading items...");
                await DownloadVerificationTestItems(level, ct);

                if (Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == level)?.VolumeTest != null)
                {
                    _testStatus.OnNext($"Running volume test...");
                    await VolumeTestManager.RunTest(_communicationClient, Instrument.VolumeTest, ct);

                    //Execute any Post test clean up methods
                    foreach (var command in _postTestCommands)
                        await command.Execute(_communicationClient, Instrument, _testStatus);
                }

                _testStatus.OnNext($"Saving test...");
                await SaveAsync();
            }
            catch (OperationCanceledException ex)
            {
                Log.Info("Test run cancelled.");
            }
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
                foreach (var validator in _validators)
                {
                    _testStatus.OnNext($"Verifying items...");
                    await validator.Execute(_communicationClient, Instrument);
                }
        }

        public void Dispose()
        {
            _communicationClient.Dispose();
            VolumeTestManager.Dispose();
        }

        private async Task DownloadVerificationTestItems(int level, CancellationToken ct)
        {
            if (!_communicationClient.IsConnected)
                await _communicationClient.Connect(ct);

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
                    (ICollection<ItemValue>)
                    await _communicationClient.GetItemValues(_communicationClient.ItemDetails.TemperatureItems());
        }

        public async Task DownloadPressureTestItems(int level)
        {
            var firstOrDefault = Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == level);
            var test = firstOrDefault?.PressureTest;
            if (test != null)
                test.Items =
                    (ICollection<ItemValue>)
                    await _communicationClient.GetItemValues(_communicationClient.ItemDetails.PressureItems());
        }
    }
}