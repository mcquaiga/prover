using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using NLog;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Communication;
using Prover.Core.DriveTypes;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;
using Prover.Core.Shared.Enums;
using Prover.Core.Storage;
using Prover.Core.VerificationTests.TestActions;
using Prover.Core.VerificationTests.VolumeVerification;
using LogManager = NLog.LogManager;

namespace Prover.Core.VerificationTests
{
    public interface IQaRunTestManager : IDisposable
    {
        Instrument Instrument { get; }

        IObservable<string> TestStatus { get; }

        Task InitializeTest(InstrumentType instrumentType, CancellationToken ct = new CancellationToken(),
            Client client = null);

        Task RunCorrectionTest(int level, CancellationToken ct = new CancellationToken());
        Task SaveAsync();
        Task RunVerifiers();
    }

    public class QaRunTestManager : IQaRunTestManager
    {
        protected static Logger Log = LogManager.GetCurrentClassLogger();
        private readonly EvcCommunicationClient _communicationClient;
        private readonly IEventAggregator _eventAggregator;
        private readonly IProverStore<Instrument> _instrumentStore;
        private readonly IReadingStabilizer _readingStabilizer;
        private readonly TachometerService _tachometerService;
        private readonly Subject<string> _testStatus = new Subject<string>();

        private readonly IEnumerable<IPreTestValidation> _validators;
        private readonly IEnumerable<IPostTestAction> _postTestCommands;

        public QaRunTestManager(
            IEventAggregator eventAggregator,
            IProverStore<Instrument> instrumentStore,
            EvcCommunicationClient commClient,
            IReadingStabilizer readingStabilizer,
            TachometerService tachometerService,
            IEnumerable<IPreTestValidation> validators = null,
            IEnumerable<IPostTestAction> postTestCommands = null)
        {
            _eventAggregator = eventAggregator;
            _instrumentStore = instrumentStore;
            _communicationClient = commClient;
            _readingStabilizer = readingStabilizer;
            _tachometerService = tachometerService;
            _validators = validators;
            _postTestCommands = postTestCommands;
        }

        public VolumeTestManagerBase VolumeTestManager { get; set; }

        public IObservable<string> TestStatus => _testStatus.AsObservable();

        public Instrument Instrument { get; private set; }

        public async Task InitializeTest(InstrumentType instrumentType, CancellationToken ct, Client client = null)
        {
            try
            {
                _communicationClient.Initialize(instrumentType);

                _testStatus.OnNext($"Connecting to {instrumentType.Name}...");
                await _communicationClient.Connect(ct);

                _testStatus.OnNext("Downloading items...");
                var items = await _communicationClient.GetItemValues(_communicationClient.ItemDetails.GetAllItemNumbers());

                _testStatus.OnNext($"Disconnecting from {instrumentType.Name}...");
                await _communicationClient.Disconnect();

                Instrument = new Instrument(instrumentType, items, client);

                if (Instrument.VolumeTest.DriveType is MechanicalDrive)
                {
                    if (SettingsManager.SettingsInstance.TestSettings.MechanicalDriveVolumeTestType ==
                        TestSettings.VolumeTestType.Automatic)
                        VolumeTestManager = new AutoVolumeTestManagerBase(_eventAggregator, _tachometerService);
                    else
                        VolumeTestManager = new ManualVolumeTestManager(_eventAggregator);
                }
                else if (Instrument.VolumeTest.DriveType is RotaryDrive)
                {
                    VolumeTestManager = new AutoVolumeTestManagerBase(_eventAggregator, _tachometerService);
                }

                await RunVerifiers();
                await SaveAsync();
            }
            catch (Exception ex)
            {
                _communicationClient.Dispose();
                Log.Error(ex);
                throw;
            }            
        }

        public async Task RunCorrectionTest(int level, CancellationToken ct)
        {
            if (Instrument == null)
                throw new NullReferenceException("Call InitializeTest before runnning a test");

            try
            {
                if (SettingsManager.SettingsInstance.TestSettings.StabilizeLiveReadings)
                {
                    _testStatus.OnNext($"Stabilizing live readings...");
                    await _readingStabilizer.WaitForReadingsToStabilizeAsync(_communicationClient, Instrument, level, ct);
                }

                await DownloadVerificationTestItems(level, ct);

                await SaveAsync();
            }
            catch (OperationCanceledException ex)
            {
                Log.Info("Test run cancelled.");
            }
        }

        public async Task RunVolumeTest(CancellationToken ct)
        {
            try
            {
                if (Instrument.VerificationTests.Any(x => x.VolumeTest != null))
                {
                    _testStatus.OnNext($"Running volume test...");
                    await VolumeTestManager.RunTest(_communicationClient, Instrument.VolumeTest, ct);

                    //Execute any Post test clean up methods
                    foreach (var command in _postTestCommands)
                        await command.Execute(_communicationClient, Instrument, _testStatus);

                    await SaveAsync();
                }
            }
            catch (OperationCanceledException e) 
            {
                Log.Info("Volume test cancelled.");
            }
        }

        public async Task SaveAsync()
        {
            try
            {
                _testStatus.OnNext($"Saving test...");

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
                    await validator.Validate(_communicationClient, Instrument);
                }
        }

        public void Dispose()
        {
            _communicationClient.Dispose();
            VolumeTestManager.Dispose();
        }

        private async Task DownloadVerificationTestItems(int level, CancellationToken ct)
        {
            _testStatus.OnNext($"Downloading items...");

            if (!_communicationClient.IsConnected)
                await _communicationClient.Connect(ct);

            if (Instrument.CompositionType == EvcCorrectorType.PTZ)
            {
                await DownloadTemperatureTestItems(level);
                await DownloadPressureTestItems(level);
            }

            if (Instrument.CompositionType == EvcCorrectorType.T)
                await DownloadTemperatureTestItems(level);

            if (Instrument.CompositionType == EvcCorrectorType.P)
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