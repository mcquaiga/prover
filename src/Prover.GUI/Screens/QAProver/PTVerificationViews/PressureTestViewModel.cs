using System;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Prover.CommProtocol.Common.Items;
using Prover.Core;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;
using ReactiveUI;

namespace Prover.GUI.Screens.QAProver.PTVerificationViews
{
    public class PressureTestViewModel : TestRunViewModelBase<Core.Models.Instruments.PressureTest>
    {
        public PressureTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            Core.Models.Instruments.PressureTest testRun) : base(screenManager, eventAggregator, testRun)
        {
            var atmChange = this.WhenAnyValue(x => x.AtmosphericGauge);
            atmChange.Subscribe(atm =>
            {
                TestRun.AtmosphericGauge = atm;
            });

            _gaugePressure = atmChange
                .Where(x => ShowAtmValues)
                .Select(x => TestRun.TotalGauge - x ?? 0)
                .ToProperty(this, x => x.GaugePressure);

            _gaugePressure = atmChange
                .Where(x => !ShowAtmValues)
                .Select(x => TestRun.TotalGauge)
                .ToProperty(this, x => x.GaugePressure);

            var gaugeChange = this.WhenAnyValue(x => x.GaugePressure);

            AtmosphericGauge = TestRun.AtmosphericGauge;
        }

        private readonly ObservableAsPropertyHelper<decimal> _gaugePressure;
        public decimal GaugePressure => _gaugePressure.Value;

        public bool ShowAtmValues => TestRun.VerificationTest.Instrument.Transducer == TransducerType.Absolute;
        public bool IsAtmGaugeReadOnly => !ShowAtmValues;        

        //private ObservableAsPropertyHelper<decimal> _gasGauge;
        //public ObservableAsPropertyHelper<decimal> GasGauge
        //{
        //    get { return _gasGauge; }
        //    set { this.RaiseAndSetIfChanged(ref _gasGauge, value); }
        //}

        //public decimal GasGaugeCal
        //{
        //    get
        //    {
        //        if (TestRun.VerificationTest.Instrument.Transducer == TransducerType.Absolute)
        //            return (TestRun.GasGauge ?? 0) - (AtmosphericGauge ?? 0);

        //        return TestRun.GasGauge ?? 0;
        //    }
        //}

        private decimal? _atmosphericGauge;
        public decimal? AtmosphericGauge
        {
            get { return _atmosphericGauge; }
            set
            {
                this.RaiseAndSetIfChanged(ref _atmosphericGauge, value);                
                EventAggregator.PublishOnUIThread(VerificationTestEvent.Raise(TestRun.VerificationTest));
            }
        }

        private decimal? _gauge;
        public decimal? Gauge
        {
            get { return _gauge; }
            set
            {
                this.RaiseAndSetIfChanged(ref _gauge, value);
                EventAggregator.PublishOnUIThread(VerificationTestEvent.Raise(TestRun.VerificationTest));
            }
        }

        public decimal? EvcGasPressure
            => TestRun.Items.GetItem(ItemCodes.Pressure.GasPressure).NumericValue
        ;

        public decimal? EvcFactor
            => TestRun.Items.GetItem(ItemCodes.Pressure.Factor).NumericValue;

        public decimal? EvcAtmPressure
            =>
                TestRun.VerificationTest.Instrument.Items.GetItem(
                    ItemCodes.Pressure.Atm).NumericValue;

        protected override void RaisePropertyChangeEvents()
        {
            NotifyOfPropertyChange(() => TestRun);
            NotifyOfPropertyChange(() => TestRun.PercentError);
            NotifyOfPropertyChange(() => TestRun.HasPassed);
            NotifyOfPropertyChange(() => EvcGasPressure);
            NotifyOfPropertyChange(() => EvcFactor);
            NotifyOfPropertyChange(() => EvcAtmPressure);
            NotifyOfPropertyChange(() => AtmosphericGauge);
            NotifyOfPropertyChange(() => PercentColour);
        }
    }
}