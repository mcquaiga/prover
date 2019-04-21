namespace Module.EvcVerification.VerificationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading;
    using System.Threading.Tasks;
    using LogManager = NLog.LogManager;


    /// <summary>
    /// Defines the <see cref="QaRunTestManager" />
    /// </summary>
    public class QaRunTestManager : IQaRunTestManager
    {

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QaRunTestManager"/> class.
        /// </summary>
        /// <param name="eventAggregator">The eventAggregator<see cref="IEventAggregator"/></param>
        /// <param name="testRunService">The testRunService<see cref="TestRunService"/></param>
        /// <param name="readingStabilizer">The readingStabilizer<see cref="IReadingStabilizer"/></param>
        /// <param name="tachometerService">The tachometerService<see cref="TachometerService"/></param>
        /// <param name="testActionsManager">The testActionsManager<see cref="ITestActionsManager"/></param>
        /// <param name="settingsService">The settingsService<see cref="ISettingsService"/></param>    
        public QaRunTestManager(
            IEventAggregator eventAggregator,
            TestRunService testRunService,
            IReadingStabilizer readingStabilizer,
            TachometerService tachometerService,
            ITestActionsManager testActionsManager,
            ISettingsService settingsService)
        {
            _eventAggregator = eventAggregator;
            _testRunService = testRunService;
            _readingStabilizer = readingStabilizer;
            _tachometerService = tachometerService;
            _settingsService = settingsService;
            TestActionsManager = testActionsManager;

            _testStatus?
                .Subscribe(s => this.Publish(new VerificationTestEvent(s)));
        }

        #endregion Public Constructors

        #region Public Properties

        public EvcCommunicationClient CommunicationClient => _communicationClient;

        /// <summary>
        /// Gets the EventAggregator
        /// </summary>
        public IEventAggregator EventAggregator { get; }

        /// <summary>
        /// Gets the Instrument
        /// </summary>
        public Instrument Instrument { get; private set; }

        /// <summary>
        /// Gets the TestStatus
        /// </summary>
        public IObservable<string> Status => _testStatus.AsObservable();

        /// <summary>
        /// Gets the TestActionsManager
        /// </summary>
        public ITestActionsManager TestActionsManager { get; }

        /// <summary>
        /// Gets or sets the VolumeTestManager
        /// </summary>
        public VolumeTestManager VolumeTestManager { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// The Dispose
        /// </summary>
        public void Dispose()
        {
            _communicationClient?.Dispose();
            _tachometerService?.Dispose();
            VolumeTestManager?.Dispose();
        }

        /// <summary>
        /// The DownloadPostVolumeTest
        /// </summary>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public Task DownloadPostVolumeTest(CancellationToken ct)
        {
            return VolumeTestManager.CompleteTest(TestActionsManager, ct);
        }

        /// <summary>
        /// The DownloadPressureTestItems
        /// </summary>
        /// <param name="level">The level<see cref="int"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task DownloadPressureTestItems(int level)
        {
            VerificationTest firstOrDefault = Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == level);
            PressureTest test = firstOrDefault?.PressureTest;
            if (test != null)
            {
                test.Items = await _communicationClient.GetPressureTestItems();
            }
        }

        /// <summary>
        /// The DownloadPreVolumeTest
        /// </summary>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public Task DownloadPreVolumeTest(CancellationToken ct)
        {
            return VolumeTestManager.PreTest(_communicationClient, Instrument.VolumeTest, TestActionsManager, ct);
        }

        /// <summary>
        /// The DownloadTemperatureTestItems
        /// </summary>
        /// <param name="levelNumber">The levelNumber<see cref="int"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task DownloadTemperatureTestItems(int levelNumber)
        {
            VerificationTest firstOrDefault = Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == levelNumber);
            TemperatureTest test = firstOrDefault?.TemperatureTest;

            if (test != null)
            {
                test.Items = await _communicationClient.GetTemperatureTestItems();
            }
        }

        public virtual async Task InitializeTest(IEvcDevice instrumentType, ICommPort commPort, ISettingsService testSettings, CancellationToken ct = new CancellationToken(), Client client = null, bool runVerifiers = true)
        {
            _communicationClient = instrumentType.CreateCommClient(commPort);           

            await _communicationClient.Connect(ct);
            IEnumerable<CommProtocol.Common.Items.ItemValue> items = await _communicationClient.GetAllItems();

            Instrument = Instrument.Create(instrumentType, items, testSettings.TestSettings, client);

            if (runVerifiers)
            {
                await TestActionsManager.ExecuteValidations(VerificationStep.PreVerification, _communicationClient, Instrument);
            }

            await _communicationClient.Disconnect();

            CreateVolumeTestManager();

            if (testSettings.Local.AutoSave)
            {
                await SaveAsync();
            }
        }

        /// <summary>
        /// The RunCorrectionTest
        /// </summary>
        /// <param name="level">The level<see cref="int"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task RunCorrectionTest(int level, CancellationToken ct)
        {
            if (Instrument == null)
            {
                throw new NullReferenceException("Call InitializeTest before runnning a test");
            }

            try
            {
                if (_settingsService.TestSettings.StabilizeLiveReadings)
                {
                    await _readingStabilizer.WaitForReadingsToStabilizeAsync(_communicationClient, Instrument, level, ct, _testStatus);
                }

                await DownloadVerificationTestItems(level, ct);

                //if (Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == level)?.VolumeTest != null)
                //{
                //    _testStatus.OnNext($"Running volume test...");
                //    await RunVolumeTest(ct);
                //}
            }
            catch (OperationCanceledException)
            {
                Log.Info("Test run cancelled.");
            }
        }

        /// <summary>
        /// The RunVolumeTest
        /// </summary>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task RunVolumeTest(CancellationToken ct)
        {
            try
            {
                await VolumeTestManager.RunFullVolumeTest(_communicationClient, Instrument.VolumeTest, TestActionsManager, ct);

                await TestActionsManager.ExecuteValidations(VerificationStep.PostVolumeVerification, _communicationClient, Instrument);

            }
            catch (OperationCanceledException)
            {               
                Log.Info("Volume test cancelled.");
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
                await _testRunService.Save(Instrument);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        #endregion Public Methods

        #region Protected Fields

        /// <summary>
        /// Defines the Log
        /// </summary>
        protected static Logger Log = LogManager.GetCurrentClassLogger();

        #endregion Protected Fields

        #region Private Fields

        /// <summary>
        /// Defines the _eventAggregator
        /// </summary>
        private readonly IEventAggregator _eventAggregator;


        /// <summary>
        /// Defines the _readingStabilizer
        /// </summary>
        private readonly IReadingStabilizer _readingStabilizer;

        /// <summary>
        /// Defines the _settingsService
        /// </summary>
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// Defines the _tachometerService
        /// </summary>
        private readonly TachometerService _tachometerService;

        /// <summary>
        /// Defines the _testRunService
        /// </summary>
        private readonly TestRunService _testRunService;

        /// <summary>
        /// Defines the _testStatus
        /// </summary>
        private readonly Subject<string> _testStatus = new Subject<string>();
        /// <summary>
        /// Defines the _communicationClient
        /// </summary>
        private EvcCommunicationClient _communicationClient;

        #endregion Private Fields

        #region Private Methods

        /// <summary>
        /// The CreateVolumeTestManager
        /// </summary>
        private void CreateVolumeTestManager()
        {

            if (Instrument.VolumeTest.DriveType is RotaryDrive)
            {
                VolumeTestManager = IoC.Get<RotaryAutoVolumeTestManager>();
            }
            else if (Instrument.VolumeTest.DriveType is MechanicalDrive)
            {
                if (_settingsService.Shared.TestSettings.MechanicalDriveVolumeTestType == TestSettings.VolumeTestType.Manual)
                {
                    VolumeTestManager = IoC.Get<ManualVolumeTestManager>();//new ManualVolumeTestManager(_eventAggregator, _communicationClient, Instrument.VolumeTest, _settingsService);
                }
                else
                {
                    VolumeTestManager = IoC.Get<MechanicalAutoVolumeTestManager>();
                }
            }
            else if (Instrument.VolumeTest.DriveType is PulseInputSensor)
            {
                VolumeTestManager = IoC.Get<FrequencyVolumeTestManager>();
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
            await _communicationClient.Connect(ct);

            if (Instrument.CompositionType == EvcCorrectorType.PTZ)
            {
                _testStatus?.OnNext($"Downloading P & T items...");
                await DownloadTemperatureTestItems(level);
                await DownloadPressureTestItems(level);
            }

            if (Instrument.CompositionType == EvcCorrectorType.T)
            {
                _testStatus?.OnNext($"Downloading T items...");
                await DownloadTemperatureTestItems(level);
            }

            if (Instrument.CompositionType == EvcCorrectorType.P)
            {
                _testStatus?.OnNext($"Downloading P items...");
                await DownloadPressureTestItems(level);
            }

            await _communicationClient.Disconnect();
        }

        #endregion Private Methods

    }
}
