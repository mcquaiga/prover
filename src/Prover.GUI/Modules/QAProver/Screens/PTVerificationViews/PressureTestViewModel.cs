using Caliburn.Micro;
using Prover.CommProtocol.Common.Items;
using Prover.Core;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace Prover.GUI.Modules.QAProver.Screens.PTVerificationViews
{
    public class PressureTestViewModel : TestRunViewModelBase<Core.Models.Instruments.PressureTest>
    {
        public PressureTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator, Core.Models.Instruments.PressureTest testRun) : base(screenManager, eventAggregator, testRun)
        {
            _atmosphericGauge = TestRun.AtmosphericGauge;

            var atmChange = this.WhenAnyValue(x => x.AtmosphericGauge);
            _gaugePressure = TestRun.GasGauge;

            this.WhenAnyValue(x => x.GaugePressure)
               .Subscribe(x => TestRun.GasGauge = x);
            this.WhenAnyValue(x => x.AtmosphericGauge)
                .Subscribe(x => TestRun.AtmosphericGauge = x);

            GaugePressure = TestRun.GasGauge;
            AtmosphericGauge = TestRun.AtmosphericGauge;
           

            //if (ShowAtmValues)
            //{
            //    _gaugePressure = atmChange
            //        .Where(x => ShowAtmValues)
            //        .Select(x => TestRun.TotalGauge != 0 ? TestRun.TotalGauge - x.Value : TestRun.GasGauge ?? 0)
            //        .ToProperty(this, x => x.GaugePressure);
            //}
            //else
            //{
            //    _gaugePressure = atmChange
            //        .Where(x => !ShowAtmValues)
            //        .Select(x => TestRun.GasGauge ?? 0)
            //        .ToProperty(this, x => x.GaugePressure);
            //}
        }

        //private readonly ObservableAsPropertyHelper<decimal> _gaugePressure;
        //public decimal GaugePressure => _gaugePressure.Value;

        private decimal? _gaugePressure;
        public decimal? GaugePressure
        {
            get { return _gaugePressure; }
            set
            {
                this.RaiseAndSetIfChanged(ref _gaugePressure, value);
                EventAggregator.PublishOnUIThread(VerificationTestEvent.Raise(TestRun.VerificationTest));
            }
        }

        public bool ShowAtmValues => TestRun.VerificationTest.Instrument.Transducer == TransducerType.Absolute;
        public bool IsAtmGaugeReadOnly => !ShowAtmValues;

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

        public decimal? EvcGasPressure => TestRun.Items.GetItem(ItemCodes.Pressure.GasPressure).NumericValue;

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