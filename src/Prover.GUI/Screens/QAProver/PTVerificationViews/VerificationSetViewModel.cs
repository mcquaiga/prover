using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Models.Instruments;
using Prover.Core.VerificationTests;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;
using Prover.GUI.Common.Screens;
using ReactiveUI;

namespace Prover.GUI.Screens.QAProver.PTVerificationViews
{
    public class VerificationSetViewModel : ViewModelBase
    {
        private CancellationTokenSource _cancellationTokenSource;

        public IQaRunTestManager QaRunTestManager;

        public VerificationSetViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
            CancelTestCommand = ReactiveCommand.Create(CancelTest);
        }

        public string Level => $"Level {VerificationTest.TestNumber + 1}";
        public bool ShowVolumeTestViewModel => VolumeTestViewModel != null;
        public TemperatureTestViewModel TemperatureTestViewModel { get; private set; }
        public PressureTestViewModel PressureTestViewModel { get; private set; }
        public SuperFactorTestViewModel SuperFactorTestViewModel { get; private set; }
        public VolumeTestViewModel VolumeTestViewModel { get; private set; }

        public VerificationTest VerificationTest { get; set; }

        public void InitializeViews(VerificationTest verificationTest, IQaRunTestManager qaTestRunTestManager = null)
        {
            VerificationTest = verificationTest;
            QaRunTestManager = qaTestRunTestManager;
            _testStatusSubscription = QaRunTestManager?.TestStatus.Subscribe(OnTestStatusChange);

            if (VerificationTest.Instrument.CompositionType == CorrectorType.PTZ)            
                SuperFactorTestViewModel = new SuperFactorTestViewModel(ScreenManager, EventAggregator, VerificationTest.SuperFactorTest);

            if (VerificationTest.Instrument.CompositionType == CorrectorType.T || VerificationTest.Instrument.CompositionType == CorrectorType.PTZ)
                TemperatureTestViewModel = new TemperatureTestViewModel(ScreenManager, EventAggregator, VerificationTest.TemperatureTest);

            if (VerificationTest.Instrument.CompositionType == CorrectorType.P ||
                VerificationTest.Instrument.CompositionType == CorrectorType.PTZ)
            {
                PressureTestViewModel = new PressureTestViewModel(ScreenManager, EventAggregator, VerificationTest.PressureTest);

                this.WhenAnyValue(x => x.PressureTestViewModel.AtmosphericGauge)
                    .Where(x => QaRunTestManager != null && x != null)
                    .Subscribe(async atm => await QaRunTestManager.SaveAsync());
            }

            if (VerificationTest.VolumeTest != null)
                VolumeTestViewModel = new VolumeTestViewModel(ScreenManager, EventAggregator, VerificationTest.VolumeTest, QaRunTestManager);
        }

        public bool ShowDownloadButton => QaRunTestManager != null;

        private bool _showProgressDialog;
        public bool ShowProgressDialog
        {
            get { return _showProgressDialog; }
            set { this.RaiseAndSetIfChanged(ref _showProgressDialog, value); }
        }

        private ReactiveCommand _cancelTestCommand;
        private IDisposable _testStatusSubscription;

        private void OnTestStatusChange(string status)
        {
            TestStatusMessage = status;
        }

        private string _testStatusMessage;
        public string TestStatusMessage
        {
            get { return _testStatusMessage; }
            set { this.RaiseAndSetIfChanged(ref _testStatusMessage, value); }
        }

        public ReactiveCommand CancelTestCommand
        {
            get { return _cancelTestCommand; }
            set { this.RaiseAndSetIfChanged(ref _cancelTestCommand, value); }
        }

        public void CancelTest()
        {
            _cancellationTokenSource?.Cancel();
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
    }
}