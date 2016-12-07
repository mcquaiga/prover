using System;
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
        public IQaRunTestManager QaRunTestManager;

        public VerificationSetViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
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

        private bool _showProgressDialog;
        public bool ShowProgressDialog
        {
            get { return _showProgressDialog; }
            set { this.RaiseAndSetIfChanged(ref _showProgressDialog, value); }
        }

        public async Task RunTest()
        {
            try
            {
                ShowProgressDialog = true;
                await QaRunTestManager.RunTest(VerificationTest.TestNumber);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occured during the verification test. See exception for details.");
            }
            finally
            {
                ShowProgressDialog = false;
                EventAggregator.PublishOnUIThread(VerificationTestEvent.Raise());
            }
        }
    }
}