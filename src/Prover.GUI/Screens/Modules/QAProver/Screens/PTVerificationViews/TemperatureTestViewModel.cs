using Caliburn.Micro;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Models.Instruments;
using Prover.GUI.Events;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using TemperatureTestRun = Prover.Core.Models.Instruments.TemperatureTest;

namespace Prover.GUI.Screens.Modules.QAProver.Screens.PTVerificationViews
{
    public class TemperatureTestViewModel : TestRunViewModelBase<TemperatureTestRun>
    {
        public decimal? EvcFactor => TestRun.Items?.GetItem(45).NumericValue;

        public decimal? EvcReading => TestRun.Items?.GetItem(26).NumericValue;

        public double Gauge { get => _gauge; set => this.RaiseAndSetIfChanged(ref _gauge, value); }

        public TemperatureTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
                                    TemperatureTestRun temperatureTest, ISubject<VerificationTest> changeObservable)
            : base(screenManager, eventAggregator, temperatureTest, changeObservable)
        {
            Gauge = temperatureTest.Gauge;
            this.WhenAnyValue(x => x.Gauge)
                .Subscribe(x =>
                {
                    TestRun.Gauge = x;
                    ChangedEvent.OnNext(TestRun.VerificationTest);
                });
        }

        protected override void RaisePropertyChangeEvents()
        {
            NotifyOfPropertyChange(() => TestRun);
            NotifyOfPropertyChange(() => PercentError);
            NotifyOfPropertyChange(() => Gauge);
            NotifyOfPropertyChange(() => EvcReading);
            NotifyOfPropertyChange(() => EvcFactor);
            NotifyOfPropertyChange(() => PercentColour);
        }

        private double _gauge;
    }
}