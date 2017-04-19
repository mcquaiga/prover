namespace Prover.Client.WPF.Screens.QAProver.PTVerificationViews
{
    public abstract class TestRunViewModelBase<T> : ViewModelBase, IHandle<VerificationTestEvent>
        where T : BaseVerificationTest
    {
        protected TestRunViewModelBase(ScreenManager screenManager, IEventAggregator eventAggregator, T testRun)
            : base(screenManager, eventAggregator)
        {
            TestRun = testRun;
            eventAggregator.Subscribe(this);
        }

        public Brush PassColour => TestRun != null && TestRun.HasPassed ? Brushes.ForestGreen : Brushes.IndianRed;

        public string PassStatusIcon => TestRun != null && TestRun.HasPassed ? "pass" : "fail";

        public Brush PercentColour
            =>
                TestRun == null || TestRun.HasPassed
                    ? Brushes.White
                    : (SolidColorBrush) new BrushConverter().ConvertFrom("#DC6156");

        public decimal? PercentError => TestRun?.PercentError;

        public T TestRun { get; set; }

        public abstract void Handle(VerificationTestEvent message);
    }
}