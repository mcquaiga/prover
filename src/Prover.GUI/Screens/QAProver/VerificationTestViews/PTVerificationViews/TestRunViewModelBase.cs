using System.Windows.Media;
using Caliburn.Micro;
using Prover.Core.Models.Instruments;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;
using Prover.GUI.Common.Screens;

namespace Prover.GUI.Screens.QAProver.VerificationTestViews.PTVerificationViews
{
    public abstract class TestRunViewModelBase<T> : ViewModelBase, IHandle<VerificationTestEvent>
        where T : BaseVerificationTest
    {
        protected TestRunViewModelBase(ScreenManager screenManager, IEventAggregator eventAggregator, T testRun)
            : base(screenManager, eventAggregator)
        {
            TestRun = testRun;
        }

        public T TestRun { get; set; }

        public decimal? PercentError => TestRun?.PercentError;

        public Brush PercentColour
            =>
            (TestRun == null) || TestRun.HasPassed
                ? Brushes.White
                : (SolidColorBrush) new BrushConverter().ConvertFrom("#DC6156");

        public Brush PassColour => (TestRun != null) && TestRun.HasPassed ? Brushes.ForestGreen : Brushes.IndianRed;

        public string PassStatusIcon => (TestRun != null) && TestRun.HasPassed ? "pass" : "fail";

        public abstract void Handle(VerificationTestEvent message);
    }
}