using Caliburn.Micro;
using Prover.Core.Models.Instruments;
using Prover.GUI.Events;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Media;

namespace Prover.GUI.Screens.Modules.QAProver.Screens.PTVerificationViews
{
    public abstract class TestRunViewModelBase<T> : ViewModelBase
        where T : IHavePercentError, IHaveVerificationTest
    {
        public ISubject<VerificationTest> ChangedEvent { get; protected set; } = new Subject<VerificationTest>();

        protected TestRunViewModelBase(ScreenManager screenManager, IEventAggregator eventAggregator, T testRun, ISubject<VerificationTest> changeObservable)
            : base(screenManager, eventAggregator)
        {
            TestRun = testRun;

            ChangedEvent
                .Subscribe(changeObservable);

            changeObservable
                .Where(vt => vt == TestRun.VerificationTest)
                .Subscribe(_ => RaisePropertyChangeEvents());
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
                    : (SolidColorBrush)new BrushConverter().ConvertFrom("#DC6156");

        public Brush PassColour => TestRun != null && TestRun.HasPassed ? Brushes.ForestGreen : Brushes.IndianRed;
        public string PassStatusIcon => TestRun != null && TestRun.HasPassed ? "pass" : "fail";
        public override void Dispose()
        {
            base.Dispose();
            ChangedEvent.OnCompleted();
            ChangedEvent = null;
        }
        protected abstract void RaisePropertyChangeEvents();
    }
}