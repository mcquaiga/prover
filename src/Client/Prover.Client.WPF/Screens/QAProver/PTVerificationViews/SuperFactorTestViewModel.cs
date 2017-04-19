namespace Prover.Client.WPF.Screens.QAProver.PTVerificationViews
{
    public class SuperFactorTestViewModel : TestRunViewModelBase<Core.Models.Instruments.SuperFactorTest>
    {
        public SuperFactorTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            Core.Models.Instruments.SuperFactorTest testRun) : base(screenManager, eventAggregator, testRun)
        {
        }

        public decimal? EVCUnsqrFactor => TestRun.EvcUnsqrFactor;

        public override void Handle(VerificationTestEvent message)
        {
            NotifyOfPropertyChange(() => TestRun);
            NotifyOfPropertyChange(() => PercentError);
            NotifyOfPropertyChange(() => EVCUnsqrFactor);
        }
    }
}