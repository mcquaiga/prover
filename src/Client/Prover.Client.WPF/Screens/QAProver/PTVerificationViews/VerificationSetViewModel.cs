namespace Prover.Client.WPF.Screens.QAProver.PTVerificationViews
{
    public class VerificationSetViewModel : ViewModelBase
    {
        public IQaRunTestManager QaRunTestManager;
        private CancellationTokenSource _cancellationTokenSource;

        private ReactiveCommand _cancelTestCommand;

        private ReactiveCommand _runTestCommand;

        private bool _showDownloadButton;

        private bool _showProgressDialog;

        private string _testStatusMessage;
        private IDisposable _testStatusSubscription;

        public VerificationSetViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
            CancelTestCommand = ReactiveCommand.Create(CancelTest);
            RunTestCommand = ReactiveCommand.CreateFromTask(RunTest);
        }

        public ReactiveCommand CancelTestCommand
        {
            get { return _cancelTestCommand; }
            set { this.RaiseAndSetIfChanged(ref _cancelTestCommand, value); }
        }

        public string Level => $"Level {VerificationTest.TestNumber + 1}";
        public PressureTestViewModel PressureTestViewModel { get; private set; }

        public ReactiveCommand RunTestCommand
        {
            get { return _runTestCommand; }
            set { this.RaiseAndSetIfChanged(ref _runTestCommand, value); }
        }

        public bool ShowDownloadButton
        {
            get { return _showDownloadButton; }
            set { this.RaiseAndSetIfChanged(ref _showDownloadButton, value); }
        }

        public bool ShowProgressDialog
        {
            get { return _showProgressDialog; }
            set { this.RaiseAndSetIfChanged(ref _showProgressDialog, value); }
        }

        public bool ShowVolumeTestViewModel => VolumeTestViewModel != null;
        public SuperFactorTestViewModel SuperFactorTestViewModel { get; private set; }
        public TemperatureTestViewModel TemperatureTestViewModel { get; private set; }

        public string TestStatusMessage
        {
            get { return _testStatusMessage; }
            set { this.RaiseAndSetIfChanged(ref _testStatusMessage, value); }
        }

        public VerificationTest VerificationTest { get; set; }
        public VolumeTestViewModel VolumeTestViewModel { get; private set; }

        public void CancelTest()
        {
            _cancellationTokenSource?.Cancel();
        }

        public void InitializeViews(VerificationTest verificationTest, IQaRunTestManager qaTestRunTestManager = null)
        {
            VerificationTest = verificationTest;
            QaRunTestManager = qaTestRunTestManager;
            ShowDownloadButton = QaRunTestManager != null;

            _testStatusSubscription = QaRunTestManager?.TestStatus.Subscribe(OnTestStatusChange);

            if (VerificationTest.Instrument.CompositionType == CorrectorType.PTZ)
            {
                TemperatureTestViewModel = new TemperatureTestViewModel(ScreenManager, EventAggregator,
                    VerificationTest.TemperatureTest);
                PressureTestViewModel = new PressureTestViewModel(ScreenManager, EventAggregator,
                    VerificationTest.PressureTest);
                SuperFactorTestViewModel = new SuperFactorTestViewModel(ScreenManager, EventAggregator,
                    VerificationTest.SuperFactorTest);
            }

            if (VerificationTest.Instrument.CompositionType == CorrectorType.T)
                TemperatureTestViewModel = new TemperatureTestViewModel(ScreenManager, EventAggregator,
                    VerificationTest.TemperatureTest);

            if (VerificationTest.Instrument.CompositionType == CorrectorType.P)
                PressureTestViewModel = new PressureTestViewModel(ScreenManager, EventAggregator,
                    VerificationTest.PressureTest);

            if (VerificationTest.VolumeTest != null)
                VolumeTestViewModel = new VolumeTestViewModel(ScreenManager, EventAggregator,
                    VerificationTest.VolumeTest);
        }

        public async Task RunTest()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            try
            {
                ShowProgressDialog = true;
                await QaRunTestManager.RunTest(VerificationTest.TestNumber, _cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An error occured during the verification test. See exception for details. {ex.Message}");
            }
            finally
            {
                ShowProgressDialog = false;
                EventAggregator.PublishOnUIThread(VerificationTestEvent.Raise());
                _cancellationTokenSource.Dispose();
            }
        }

        private void OnTestStatusChange(string status)
        {
            TestStatusMessage = status;
        }
    }
}