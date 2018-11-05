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
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.DriveTypes;
using Prover.Core.ExternalIntegrations.Validators;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;
using Prover.Core.Storage;
using Prover.Core.VerificationTests.TestActions;
using Prover.Core.VerificationTests.VolumeVerification;
using Splat;
using LogManager = NLog.LogManager;

namespace Prover.Core.VerificationTests
{ 
    public static class TestRunManager
    {        
        public static IQaRunTestManager MiniAtTestManager {get; set;}
        public static IQaRunTestManager TocTestManager { get; private set; }

        public static async Task<IQaRunTestManager> CreateTestRun(InstrumentType instrumentType, CommPort commPort, Action<string> statusAction = null)
        {
            var qaTestRunManager = (IQaRunTestManager)Locator.Current.GetService<IQaRunTestManager>();
            qaTestRunManager.TestStatus.Subscribe(statusAction);

            if (instrumentType == Instruments.Toc)
            {
                MiniAtTestManager = qaTestRunManager;
                instrumentType = Instruments.MiniAt;
            }

            await qaTestRunManager.InitializeTest(instrumentType, commPort);                     

            return qaTestRunManager;
        }

        public static async Task<IQaRunTestManager> CreateNextTestRun(InstrumentType instrumentType, CommPort commPort, Action<string> statusAction = null)
        {
            if (instrumentType == Instruments.Toc)
            {
                TocTestManager = (IQaRunTestManager)Locator.Current.GetService<IQaRunTestManager>();
                TocTestManager.TestStatus.Subscribe(statusAction);

                await TocTestManager.InitializeTest(instrumentType, commPort); 
                
                MiniAtTestManager.Instrument.LinkedTest = TocTestManager.Instrument;
                MiniAtTestManager.Instrument.LinkedTestId = TocTestManager.Instrument.Id;

                return TocTestManager;
            }

            return null;
        }
    }

    public class QaRunTestManager : IQaRunTestManager
    {
        private const int VolumeTestNumber = 0;
        protected static Logger Log = LogManager.GetCurrentClassLogger();
        private EvcCommunicationClient _communicationClient;
        private readonly IInstrumentStore<Instrument> _instrumentStore;
        private readonly IReadingStabilizer _readingStabilizer;
        private readonly IEnumerable<IValidator> _validators;
        private readonly Subject<string> _testStatus = new Subject<string>();

        public QaRunTestManager(
            IEventAggregator eventAggregator,
            IInstrumentStore<Instrument> instrumentStore,
            IReadingStabilizer readingStabilizer,
            VolumeTestManager volumeTestManager,            
            IEnumerable<IValidator> validators,
            ITestActionsManager testActionsManager )
        {
            VolumeTestManager = volumeTestManager;
            EventAggregator = eventAggregator;
            _instrumentStore = instrumentStore;           
            _readingStabilizer = readingStabilizer;
            _validators = validators;      
            TestActionsManager = testActionsManager;
        }

        public IObservable<string> TestStatus => _testStatus.AsObservable();

        public IEventAggregator EventAggregator { get; }

        public VolumeTestManager VolumeTestManager { get; private set; }

        public void Dispose()
        {
            Task.Run(SaveAsync);
            _communicationClient?.Dispose();
            VolumeTestManager?.Dispose();
        }

        public Instrument Instrument { get; private set; }
        public ITestActionsManager TestActionsManager { get; }

        public async Task InitializeTest(InstrumentType instrumentType, CommPort commPort)
        {
            _communicationClient = instrumentType.ClientFactory.Invoke(commPort);

            _testStatus.OnNext($"Connecting to {instrumentType.Name}...");
            await _communicationClient.Connect();

            _testStatus.OnNext("Downloading items...");
            var items = await _communicationClient.GetAllItems();

            Instrument = new Instrument(instrumentType, items);          

            await TestActionsManager.RunVerificationInitActions(_communicationClient, Instrument);

            await RunVerifier();

            _testStatus.OnNext($"Disconnecting from {instrumentType.Name}...");
            await _communicationClient.Disconnect();

            if (Instrument.VolumeTest.DriveType is PulseInputSensor)
            {
                VolumeTestManager = new FrequencyVolumeTestManager(EventAggregator);
            }

            await SaveAsync();
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
                    await VolumeTestManager.RunFullVolumeTest(_communicationClient, Instrument.VolumeTest, TestActionsManager, ct);
                }

                _testStatus.OnNext($"Saving test...");
                await SaveAsync();
            }
            catch (OperationCanceledException ex)
            {
                Log.Info("Test run cancelled.");
            }            
        }

        public async Task DownloadPostVolumeTest(CancellationToken ct = new CancellationToken())
        {
            await VolumeTestManager.CompleteTest(_communicationClient, Instrument.VolumeTest, TestActionsManager, ct,  false);
        }

        public async Task DownloadPreVolumeTest()
        {
            await VolumeTestManager.InitializeTest(_communicationClient, Instrument.VolumeTest, TestActionsManager);
        }

        private async Task DownloadVerificationTestItems(int level, CancellationToken ct)
        {
            if (!_communicationClient.IsConnected)
                await _communicationClient.Connect();

            switch (Instrument.CompositionType)
            {
                case CorrectorType.PTZ:
                    await DownloadTemperatureTestItems(level);
                    await DownloadPressureTestItems(level);
                    break;
                case CorrectorType.T:
                    await DownloadTemperatureTestItems(level);
                    break;
                case CorrectorType.P:
                    await DownloadPressureTestItems(level);
                    break;
            }

            await _communicationClient.Disconnect();
        }

        public async Task DownloadTemperatureTestItems(int levelNumber)
        {
            var firstOrDefault = Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == levelNumber);
            var test = firstOrDefault?.TemperatureTest;

            if (test != null)
                test.Items =
                    await _communicationClient.GetTemperatureTestItems();
        }

        public async Task DownloadPressureTestItems(int level)
        {
            var firstOrDefault = Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == level);
            var test = firstOrDefault?.PressureTest;
            if (test != null)
                test.Items = await _communicationClient.GetPressureTestItems();
        }

        public async Task SaveAsync()
        {
            try
            {
                if (Instrument != null)
                    await _instrumentStore.UpsertAsync(Instrument);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public async Task RunVerifier()
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