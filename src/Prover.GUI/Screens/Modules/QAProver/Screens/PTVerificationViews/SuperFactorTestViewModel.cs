using Caliburn.Micro;

namespace Prover.GUI.Screens.Modules.QAProver.Screens.PTVerificationViews
{
    public class SuperFactorTestViewModel : TestRunViewModelBase<Core.Models.Instruments.SuperFactorTest>
    {
        public SuperFactorTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            Core.Models.Instruments.SuperFactorTest testRun) : base(screenManager, eventAggregator, testRun)
        {
        }

        public decimal? EVCUnsqrFactor => TestRun.EvcUnsqrFactor;

        protected override void RaisePropertyChangeEvents()
        {
            NotifyOfPropertyChange(() => TestRun);
            NotifyOfPropertyChange(() => PercentError);
            NotifyOfPropertyChange(() => EVCUnsqrFactor);
        }
    }
}