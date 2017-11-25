using System.Windows.Media;
using Caliburn.Micro;
using Prover.Core.Models.Instruments;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;
using Prover.GUI.Common.Screens;
using ReactiveUI;

namespace Prover.GUI.Modules.QAProver.Screens.PTVerificationViews
{
    public abstract class TestRunViewModelBase<T> : ViewModelBase, IHandle<VerificationTestEvent>
        where T : IHavePercentError, IHaveVerificationTest
    {
        protected TestRunViewModelBase(ScreenManager screenManager, IEventAggregator eventAggregator, T testRun)
            : base(screenManager, eventAggregator)
        {
            TestRun = testRun;
            eventAggregator.Subscribe(this);
        }

        private T _testRun;

        public T TestRun
        {
            get => _testRun;
            set => this.RaiseAndSetIfChanged(ref _testRun, value);
        }

        public decimal? PercentError => TestRun?.PercentError;

        public Brush PercentColour
            =>
                TestRun == null || TestRun.HasPassed
                    ? Brushes.White
                    : (SolidColorBrush) new BrushConverter().ConvertFrom("#DC6156");

        public Brush PassColour => TestRun != null && TestRun.HasPassed ? Brushes.ForestGreen : Brushes.IndianRed;

        public string PassStatusIcon => TestRun != null && TestRun.HasPassed ? "pass" : "fail";

        public virtual void Handle(VerificationTestEvent message)
        {
            if (message.VerificationTest == TestRun.VerificationTest)
            {
                RaisePropertyChangeEvents();
            }
        }

        protected abstract void RaisePropertyChangeEvents();
    }
}