namespace Prover.Core.VerificationTests
{
    using Caliburn.Micro;
    using NLog;
    using Prover.CommProtocol.Common;
    using Prover.CommProtocol.Common.IO;
    using Prover.CommProtocol.Common.Models.Instrument;
    using Prover.Core.DriveTypes;
    using Prover.Core.ExternalDevices;
    using Prover.Core.Models.Clients;
    using Prover.Core.Models.Instruments;
    using Prover.Core.Models.Instruments.DriveTypes;
    using Prover.Core.Services;
    using Prover.Core.Settings;
    using Prover.Core.Shared.Enums;
    using Prover.Core.VerificationTests.Events;
    using Prover.Core.VerificationTests.TestActions;
    using Prover.Core.VerificationTests.VolumeVerification;
    using PubSub.Extension;
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
        #region Fields

        /// <summary>
        /// Defines the _eventAggregator
        /// </summary>
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// Defines the _postTestCommands
        /// </summary>
        private readonly IEnumerable<IPostTestAction> _postTestCommands;

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
        /// Defines the _validators
        /// </summary>
        private readonly IEnumerable<IPreTestValidation> _validators;

        /// <summary>
        /// Defines the Log
        /// </summary>
        protected static Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Defines the _communicationClient
        /// </summary>
        private EvcCommunicationClient _communicationClient;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QaRunTestManager"/> class.
        /// </summary>
        /// <param name="eventAggregator">The eventAggregator<see cref="IEventAggregator"/></param>
        /// <param name="testRunService">The testRunService<see cref="TestRunService"/></param>
        /// <param name="readingStabilizer">The readingStabilizer<see cref="IReadingStabilizer"/></param>
        /// <param name="tachometerService">The tachometerService<see cref="TachometerService"/></param>
        /// <param name="testActionsManager">The testActionsManager<see cref="ITestActionsManager"/></param>
        /// <param name="settingsService">The settingsService<see cref="ISettingsService"/></param>
        /// <param name="validators">The validators<see cref="IEnumerable{IPreTestValidation}"/></param>
        /// <param name="postTestCommands">The postTestCommands<see cref="IEnumerable{IPostTestAction}"/></param>
        public QaRunTestManager(
            IEventAggregator eventAggregator,
            TestRunService testRunService,
            IReadingStabilizer readingStabilizer,
            TachometerService tachometerService,
            ITestActionsManager testActionsManager,
            ISettingsService settingsService,
            IEnumerable<IPreTestValidation> validators = null,
            IEnumerable<IPostTestAction> postTestCommands = null)
        {
            _eventAggregator = eventAggregator;
            _testRunService = testRunService;
            _readingStabilizer = readingStabilizer;
            _tachometerService = tachometerService;
            _settingsService = settingsService;
            TestActionsManager = testActionsManager;
            _validators = validators;
            _postTestCommands = postTestCommands;

            _testStatus.Subscribe(s => this.Publish(new VerificationTestEvent(s)));
        }

        #endregion

        #region Properties

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
        public IObservable<string> Status => _testStatus.AsObservable();

        /// <summary>
        /// Gets or sets the VolumeTestManager
        /// </summary>
        public VolumeTestManager VolumeTestManager { get; set; }

        public EvcCommunicationClient CommunicationClient => _communicationClient;

        #endregion

        #region Methods

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
        public async Task DownloadPostVolumeTest(CancellationToken ct)
        {
            await VolumeTestManager.CompleteTest(TestActionsManager, ct);
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
                test.Items = await _communicationClient.GetPressureTestItems();
        }

        /// <summary>
        /// The DownloadPreVolumeTest
        /// </summary>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task DownloadPreVolumeTest(CancellationToken ct)
        {
            await VolumeTestManager.PreTest(_communicationClient, Instrument.VolumeTest, TestActionsManager, ct);
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
                    await _communicationClient.GetTemperatureTestItems();
        }

        /// <summary>
        /// The InitializeTest
        /// </summary>
        /// <param name="instrumentType">The instrumentType<see cref="EvcDevice"/></param>
        /// <param name="commPort">The commPort<see cref="ICommPort"/></param>
        /// <param name="testSettings">The testSettings<see cref="TestSettings"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <param name="client">The client<see cref="Client"/></param>      
        /// <returns>The <see cref="Task"/></returns>
        public async Task InitializeTest(IEvcDevice instrumentType, ICommPort commPort, ISettingsService testSettings,
            CancellationToken ct = new CancellationToken(), Client client = null, bool runVerifiers = true)
        {       
            await GetInstrument(instrumentType, commPort, testSettings, client, ct);

            if (runVerifiers)
            {
                await RunVerifiers();
                await TestActionsManager.RunVerificationInitActions(_communicationClient, Instrument);
            }        

            await _communicationClient.Disconnect();

            CreateVolumeTestManager();
            await SaveAsync();
        }

        private async Task GetInstrument(InstrumentType instrumentType, ICommPort commPort, ISettingsService testSettings, Client client, CancellationToken ct)
        {
            _communicationClient = EvcCommunicationClient.Create(instrumentType, commPort);
            _communicationClient.Status.Subscribe(_testStatus);

            await _communicationClient.Connect(ct);
            var items = await _communicationClient.GetAllItems();

            Instrument = Instrument.Create(instrumentType, items, testSettings.TestSettings, client);
            
            await RunVerifiers();
            await TestActionsManager.RunVerificationInitActions(_communicationClient, Instrument);

            await _communicationClient.Disconnect();

            CreateVolumeTestManager();

            if (testSettings.Local.AutoSave)
                await SaveAsync();
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
                throw new NullReferenceException("Call InitializeTest before runnning a test");

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
        /// The RunVerifiers
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        public async Task RunVerifiers()
        {
            if (_validators?.Any() == true)
                foreach (var validator in _validators)
                {
                    _testStatus.OnNext($"Verifying items...");
                    await validator.Validate(_communicationClient, Instrument);
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
                VolumeTestManager.StatusMessage.Subscribe(_testStatus);
                await VolumeTestManager.RunFullVolumeTest(_communicationClient, Instrument.VolumeTest, TestActionsManager, ct);

                //Execute any Post test clean up methods
                foreach (var command in _postTestCommands)
                    await command.Execute(_communicationClient, Instrument, _testStatus);
              
            }
            catch (OperationCanceledException)
            {
                _testStatus.OnNext($"Volume test cancelled.");
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
                _testStatus.OnNext($"Saving test...");

                await _testRunService.Save(Instrument);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

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
                    VolumeTestManager = IoC.Get<ManualVolumeTestManager>();//new ManualVolumeTestManager(_eventAggregator, _communicationClient, Instrument.VolumeTest, _settingsService);
                else
                    VolumeTestManager = IoC.Get<MechanicalAutoVolumeTestManager>();
            }
            else if (Instrument.VolumeTest.DriveType is PulseInputSensor)
            {
                VolumeTestManager = IoC.Get<ManualVolumeTestManager>();
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
                _testStatus.OnNext($"Downloading P & T items...");
                await DownloadTemperatureTestItems(level);
                await DownloadPressureTestItems(level);
            }

            if (Instrument.CompositionType == EvcCorrectorType.T)
            {
                _testStatus.OnNext($"Downloading T items...");
                await DownloadTemperatureTestItems(level);
            }

            if (Instrument.CompositionType == EvcCorrectorType.P)
            {
                _testStatus.OnNext($"Downloading P items...");
                await DownloadPressureTestItems(level);
            }

            await _communicationClient.Disconnect();
        }  

        #endregion
    } 
}
