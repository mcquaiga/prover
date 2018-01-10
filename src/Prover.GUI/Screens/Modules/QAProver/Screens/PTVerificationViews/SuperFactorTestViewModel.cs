using Caliburn.Micro;

namespace Prover.GUI.Screens.Modules.QAProver.Screens.PTVerificationViews
{
    public class SuperFactorTestViewModel : TestRunViewModelBase<Core.Models.Instruments.SuperFactorTest>
    {
        public SuperFactorTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            Core.Models.Instruments.SuperFactorTest testRun) : base(screenManager, eventAggregator, testRun)
        {
        }

        private decimal _lastGaugeTemp;
        private decimal _lastGaugePressure;

        protected override void RaisePropertyChangeEvents()
        {
            if (_lastGaugeTemp != TestRun.GaugeTemp || _lastGaugePressure != TestRun.GaugePressure.Value)
                TestRun.Calculate();

            _lastGaugeTemp = TestRun.GaugeTemp;
            _lastGaugePressure = TestRun.GaugePressure ?? 0;
            
            NotifyOfPropertyChange(() => TestRun);
            NotifyOfPropertyChange(() => PercentError);          
        }
    }
}