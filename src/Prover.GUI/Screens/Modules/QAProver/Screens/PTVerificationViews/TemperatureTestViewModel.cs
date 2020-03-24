using Caliburn.Micro;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Models.Instruments;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using TemperatureTestRun = Prover.Core.Models.Instruments.TemperatureTest;

namespace Prover.GUI.Screens.Modules.QAProver.Screens.PTVerificationViews
{
    public class TemperatureTestViewModel : TestRunViewModelBase<TemperatureTestRun>
    {
        

        public TemperatureTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            TemperatureTestRun temperatureTest, ISubject<VerificationTest> changeObservable)
            : base(screenManager, eventAggregator, temperatureTest, changeObservable)
        {          

            this.WhenAnyValue(x => x.Gauge)
                .Subscribe(x =>
                {
                    TestRun.Gauge = x;
                    ChangedEvent.OnNext(TestRun.VerificationTest);
                });

        }

        private double _gauge;
        public double Gauge { get => _gauge; set => this.RaiseAndSetIfChanged(ref _gauge, value); }

        public decimal? EvcReading => TestRun.Items?.GetItem(26).NumericValue;
        public decimal? EvcFactor => TestRun.Items?.GetItem(45).NumericValue;

        protected override void RaisePropertyChangeEvents()
        {
            NotifyOfPropertyChange(() => TestRun);
            NotifyOfPropertyChange(() => PercentError);
            NotifyOfPropertyChange(() => Gauge);
            NotifyOfPropertyChange(() => EvcReading);
            NotifyOfPropertyChange(() => EvcFactor);
            NotifyOfPropertyChange(() => PercentColour);
        }
    }
}