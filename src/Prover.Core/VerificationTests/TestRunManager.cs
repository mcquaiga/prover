namespace Prover.Core.VerificationTests
{
    using Caliburn.Micro;
    using NLog;
    using Prover.CommProtocol.Common;
    using Prover.CommProtocol.Common.IO;
    using Prover.Core.DriveTypes;
    using Prover.Core.ExternalIntegrations.Validators;
    using Prover.Core.Models.Instruments;
    using Prover.Core.Storage;
    using Prover.Core.VerificationTests.VolumeVerification;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading;
    using System.Threading.Tasks;
    using LogManager = NLog.LogManager;

    /// <summary>
    /// Defines the <see cref="TestRunManager" />
    /// </summary>
    public class TestRunManager : ITestRunManager
    {
        #region Constants

        /// <summary>
        /// Defines the VolumeTestNumber
        /// </summary>
        private const int VolumeTestNumber = 0;

        #endregion

        #region Fields

        /// <summary>
        /// Defines the _instrumentStore
        /// </summary>
        private readonly IInstrumentStore<Instrument> _instrumentStore;

        /// <summary>
        /// Defines the _readingStabilizer
        /// </summary>
        private readonly IReadingStabilizer _readingStabilizer;

        /// <summary>
        /// Defines the _testStatus
        /// </summary>
        private readonly Subject<string> _testStatus = new Subject<string>();

        /// <summary>
        /// Defines the _validators
        /// </summary>
        private readonly IEnumerable<IValidator> _validators;

        /// <summary>
        /// Defines the Log
        /// </summary>
        protected static Logger Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestRunManager"/> class.
        /// </summary>
        /// <param name="eventAggregator">The eventAggregator<see cref="IEventAggregator"/></param>
        /// <param name="instrumentStore">The instrumentStore<see cref="IInstrumentStore{Instrument}"/></param>
        /// <param name="readingStabilizer">The readingStabilizer<see cref="IReadingStabilizer"/></param>
        /// <param name="volumeTestManager">The volumeTestManager<see cref="VolumeTestManager"/></param>
        /// <param name="validators">The validators<see cref="IEnumerable{IValidator}"/></param>
        /// <param name="testActionsManager">The testActionsManager<see cref="ITestActionsManager"/></param>
        public TestRunManager(
            IEventAggregator eventAggregator,
            IInstrumentStore<Instrument> instrumentStore,
            IReadingStabilizer readingStabilizer,
            VolumeTestManager volumeTestManager,
            IEnumerable<IValidator> validators,
            ITestActionsManager testActionsManager)
        {
            VolumeTestManager = volumeTestManager;
            EventAggregator = eventAggregator;
            _instrumentStore = instrumentStore;
            _readingStabilizer = readingStabilizer;
            _validators = validators;
            TestActionsManager = testActionsManager;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the CommunicationClient
        /// </summary>
        public EvcCommunicationClient CommunicationClient { get; private set; }

        /// <summary>
        /// Gets the EventAggregator
        /// </summary>
        public IEventAggregator EventAggregator { get; }

        /// <summary>
        /// Gets the Instrument
        /// </summary>
        public Instrument Instrument { get; private set; }

        /// <summary>
        /// Gets the TestActionsManager
        /// </summary>
        public ITestActionsManager TestActionsManager { get; }

        /// <summary>
        /// Gets the TestStatus
        /// </summary>
        public IObservable<string> TestStatus => _testStatus.AsObservable();

        /// <summary>
        /// Gets the VolumeTestManager
        /// </summary>
        public VolumeTestManager VolumeTestManager { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// The Dispose
        /// </summary>
        public void Dispose()
        {
            Task.Run(SaveAsync);
            CommunicationClient?.Dispose();
            VolumeTestManager?.Dispose();
        }

        /// <summary>
        /// The DownloadPostVolumeTest
        /// </summary>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task DownloadPostVolumeTest(CancellationToken ct = new CancellationToken())
        {
            await VolumeTestManager.CompleteTest(CommunicationClient, Instrument.VolumeTest, TestActionsManager, ct, false);
        }

        /// <summary>
        /// The DownloadPressureTestItems
        /// </summary>
        /// <param name="level">The level<see cref="int"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task DownloadPressureTestItems(int level)
        {
            var firstOrDefault = Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == level);
            var test = firstOrDefault?.PressureTest;
            if (test != null)
                test.Items = await CommunicationClient.GetPressureTestItems();
        }

        /// <summary>
        /// The DownloadPreVolumeTest
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        public async Task DownloadPreVolumeTest()
        {
            await VolumeTestManager.InitializeTest(CommunicationClient, Instrument.VolumeTest, TestActionsManager);
        }

        /// <summary>
        /// The DownloadTemperatureTestItems
        /// </summary>
        /// <param name="levelNumber">The levelNumber<see cref="int"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task DownloadTemperatureTestItems(int levelNumber)
        {
            var firstOrDefault = Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == levelNumber);
            var test = firstOrDefault?.TemperatureTest;

            if (test != null)
                test.Items =
                    await CommunicationClient.GetTemperatureTestItems();
        }

        /// <summary>
        /// The InitializeTest
        /// </summary>
        /// <param name="instrumentType">The instrumentType<see cref="InstrumentType"/></param>
        /// <param name="commPort">The commPort<see cref="CommPort"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task InitializeTest(InstrumentType instrumentType, CommPort commPort)
        {
            await InitializeTest(instrumentType, commPort, null);
        }

        /// <summary>
        /// The InitializeTest
        /// </summary>
        /// <param name="instrumentType">The instrumentType<see cref="InstrumentType"/></param>
        /// <param name="commPort">The commPort<see cref="CommPort"/></param>
        /// <param name="setItemValues">The setItemValues<see cref="Dictionary{int, int}"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task InitializeTest(InstrumentType instrumentType, CommPort commPort, Dictionary<int, decimal> setItemValues)
        {
            CommunicationClient = instrumentType.ClientFactory.Invoke(commPort);

            _testStatus.OnNext($"Connecting to {instrumentType.Name}...");
            await CommunicationClient.Connect();

            if (setItemValues != null)
            {
                foreach (var item in setItemValues)
                {
                    await CommunicationClient.SetItemValue(item.Key, item.Value);
                }
            }            

            _testStatus.OnNext("Downloading items...");
            var items = await CommunicationClient.GetAllItems();

            Instrument = new Instrument(instrumentType, items);

            await TestActionsManager.RunVerificationInitActions(CommunicationClient, Instrument);

            await RunVerifier();

            _testStatus.OnNext($"Disconnecting from {instrumentType.Name}...");
            await CommunicationClient.Disconnect();

            if (Instrument.VolumeTest.DriveType is PulseInputSensor)
            {
                VolumeTestManager = new FrequencyVolumeTestManager(EventAggregator);
            }

            await SaveAsync();
        }

        /// <summary>
        /// The RunTest
        /// </summary>
        /// <param name="level">The level<see cref="int"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task RunTest(int level, CancellationToken ct)
        {
            try
            {
                if (Instrument == null) throw new NullReferenceException("Call InitializeTest before runnning a test");

                _testStatus.OnNext($"Stabilizing live readings...");
                await _readingStabilizer.WaitForReadingsToStabilizeAsync(CommunicationClient, Instrument, level, ct);

                _testStatus.OnNext($"Downloading items...");
                await DownloadVerificationTestItems(level, ct);

                if (Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == level)?.VolumeTest != null)
                {
                    _testStatus.OnNext($"Running volume test...");
                    await VolumeTestManager.RunFullVolumeTest(CommunicationClient, Instrument.VolumeTest, TestActionsManager, ct);
                }

                _testStatus.OnNext($"Saving test...");
                await SaveAsync();
            }
            catch (OperationCanceledException ex)
            {
                Log.Info("Test run cancelled.");
            }
        }

        /// <summary>
        /// The RunVerifier
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        public async Task RunVerifier()
        {
            if (_validators != null && _validators.Any())
            {
                foreach (var validator in _validators)
                {
                    _testStatus.OnNext($"Verifying items...");
                    await validator.Validate(CommunicationClient, Instrument);
                }
            }
        }

        /// <summary>
        /// The SaveAsync
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
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

        /// <summary>
        /// The DownloadVerificationTestItems
        /// </summary>
        /// <param name="level">The level<see cref="int"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private async Task DownloadVerificationTestItems(int level, CancellationToken ct)
        {
            if (!CommunicationClient.IsConnected)
                await CommunicationClient.Connect();

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

            await CommunicationClient.Disconnect();
        }

        #endregion
    }
}
