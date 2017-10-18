using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using Prover.Core.Models.Instruments;
using Prover.Core.Shared.Enums;
using Prover.Core.VerificationTests;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;
using Prover.GUI.Common.Screens;
using Prover.GUI.Common.Screens.Dialogs;
using ReactiveUI;

namespace Prover.GUI.Modules.QAProver.Screens.PTVerificationViews
{
    public class VerificationSetViewModel : ViewModelBase, IDisposable
    {
        public VerificationSetViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {                   
            RunTestCommand =
                DialogDisplayHelpers.ProgressStatusDialogCommand(eventAggregator, "Downloading data...", RunTest);
        }

        public IQaRunTestManager QaRunTestManager;

        public ColorZoneMode HeaderZoneColor
            => VerificationTest.TestNumber == 0 ? ColorZoneMode.PrimaryDark : ColorZoneMode.Accent;

        public Brush HeaderColour
            => VerificationTest.TestNumber == 0
                ? new SolidColorBrush(Colors.DarkRed)
                : new SolidColorBrush(Colors.Orange);

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

            ShowDownloadButton = QaRunTestManager != null;            

            if (VerificationTest.Instrument.CompositionType == EvcCorrectorType.PTZ)
                SuperFactorTestViewModel =
                    new SuperFactorTestViewModel(ScreenManager, EventAggregator, VerificationTest.SuperFactorTest);

            if (VerificationTest.Instrument.CompositionType == EvcCorrectorType.T ||
                VerificationTest.Instrument.CompositionType == EvcCorrectorType.PTZ)
            {
                TemperatureTestViewModel =
                    new TemperatureTestViewModel(ScreenManager, EventAggregator, VerificationTest.TemperatureTest);

                //this.WhenAnyValue(x => x.TemperatureTestViewModel.Gauge)
                //    .Where(x => QaRunTestManager != null)
                //    .Subscribe(async y => await QaRunTestManager.SaveAsync());
            }

            if (VerificationTest.Instrument.CompositionType == EvcCorrectorType.P ||
                VerificationTest.Instrument.CompositionType == EvcCorrectorType.PTZ)
            {
                PressureTestViewModel =
                    new PressureTestViewModel(ScreenManager, EventAggregator, VerificationTest.PressureTest);

                //this.WhenAnyValue(x => x.PressureTestViewModel.AtmosphericGauge,
                //        x => x.PressureTestViewModel.GaugePressure)
                //    .Where(x => QaRunTestManager != null)
                //    .Subscribe(async atm => await QaRunTestManager.SaveAsync());
            }

            if (VerificationTest.VolumeTest != null)
            {
                VolumeTestViewModel =
                    new VolumeTestViewModel(ScreenManager, EventAggregator, VerificationTest.VolumeTest, QaRunTestManager);                
            }
        }
        public void Dispose()
        {
            SuperFactorTestViewModel?.TryClose();
            TemperatureTestViewModel?.TryClose();
            PressureTestViewModel?.TryClose();
            VolumeTestViewModel?.TryClose();
            _cancelTestCommand?.Dispose();
            _runTestCommand?.Dispose();
        }


        #region Properties

        private bool _showDownloadButton;

        public bool ShowDownloadButton
        {
            get => _showDownloadButton;
            set => this.RaiseAndSetIfChanged(ref _showDownloadButton, value);
        }

        private bool _showProgressDialog;

        public bool ShowProgressDialog
        {
            get => _showProgressDialog;
            set => this.RaiseAndSetIfChanged(ref _showProgressDialog, value);
        }        

        private string _testStatusMessage;

        public string TestStatusMessage
        {
            get => _testStatusMessage;
            set => this.RaiseAndSetIfChanged(ref _testStatusMessage, value);
        }

        private ReactiveCommand _cancelTestCommand;
        public ReactiveCommand CancelTestCommand
        {
            get => _cancelTestCommand;
            set => this.RaiseAndSetIfChanged(ref _cancelTestCommand, value);
        }

        private ReactiveCommand _runTestCommand;

        public ReactiveCommand RunTestCommand
        {
            get => _runTestCommand;
            set => this.RaiseAndSetIfChanged(ref _runTestCommand, value);
        }

        #endregion
       
        public async Task RunTest(IObserver<string> statusObserver, CancellationToken cancellationToken)
        {            
            try
            {
                QaRunTestManager.TestStatus.Subscribe(statusObserver);
                await QaRunTestManager.RunCorrectionTest(VerificationTest.TestNumber, cancellationToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex,
                    $"An error occured during the verification test. See exception for details. {ex.Message}");
            }
            finally
            {               
                EventAggregator.PublishOnUIThread(VerificationTestEvent.Raise(VerificationTest));                
            }
        }

       
    }
}