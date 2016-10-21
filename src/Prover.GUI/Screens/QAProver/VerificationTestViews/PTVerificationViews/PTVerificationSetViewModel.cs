using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Instruments;
using Prover.Core.VerificationTests;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;
using Prover.GUI.Common.Screens;

namespace Prover.GUI.Screens.QAProver.VerificationTestViews.PTVerificationViews
{
    public class VerificationSetViewModel : ViewModelBase
    {
        private readonly QaRunTestManager _qaRunTestManager;

        public VerificationSetViewModel(ScreenManager screenManager, IEventAggregator eventAggregator, VerificationTest verificationTest, QaRunTestManager qaRunTestManager = null)
            : base(screenManager, eventAggregator)
        {
            _qaRunTestManager = qaRunTestManager;

            VerificationTest = verificationTest;
            CreateViews();
        }

        public string Level => $"Level {VerificationTest.TestNumber + 1}";
        public bool ShowVolumeTestViewModel => VolumeTestViewModel != null;
        public TemperatureTestViewModel TemperatureTestViewModel { get; private set; }
        public PressureTestViewModel PressureTestRun { get; private set; }
        public SuperFactorTestViewModel SuperFactorViewModelFactor { get; private set; }
        public VolumeTestViewModel VolumeTestViewModel { get; private set; }

        public VerificationTest VerificationTest { get; }

        private void CreateViews()
        {
            if (VerificationTest.Instrument.CompositionType == CorrectorType.PTZ)
            {
                TemperatureTestViewModel = new TemperatureTestViewModel(ScreenManager, EventAggregator, VerificationTest.TemperatureTest);
                PressureTestRun = new PressureTestViewModel(ScreenManager, EventAggregator, VerificationTest.PressureTest);
                SuperFactorViewModelFactor = new SuperFactorTestViewModel(ScreenManager, EventAggregator, VerificationTest.SuperFactorTest);
            }

            if (VerificationTest.Instrument.CompositionType == CorrectorType.T)
            {
                TemperatureTestViewModel = new TemperatureTestViewModel(ScreenManager, EventAggregator, VerificationTest.TemperatureTest);
            }

            if (VerificationTest.Instrument.CompositionType == CorrectorType.P)
            {
                PressureTestRun = new PressureTestViewModel(ScreenManager, EventAggregator, VerificationTest.PressureTest);
            }

            if (VerificationTest.VolumeTest != null)
            {
                VolumeTestViewModel = new VolumeTestViewModel(ScreenManager, EventAggregator, VerificationTest.VolumeTest);
            }
        }

        public async Task RunTest()
        {
            await _qaRunTestManager.RunTest(VerificationTest.TestNumber);
            EventAggregator.PublishOnUIThread(VerificationTestEvent.Raise());
        }
    }
}