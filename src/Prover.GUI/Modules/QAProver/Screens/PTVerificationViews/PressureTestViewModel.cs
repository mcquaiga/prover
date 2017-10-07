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
            _gaugePressure = TestRun.GasGauge;
            _atmosphericGauge = TestRun.AtmosphericGauge;

            this.WhenAnyValue(x => x.GaugePressure, x => x.AtmosphericGauge, 
                    (g, a) => g + a)
                .ToProperty(this, x => x.AbsolutePressure, out _absolutePressure);

            this.WhenAnyValue(x => x.GaugePressure)
                .Subscribe(x => TestRun.GasGauge = x);

            this.WhenAnyValue(x => x.AtmosphericGauge)
                .Subscribe(x => TestRun.AtmosphericGauge = x);

            GaugePressure = TestRun.GasGauge;
            AtmosphericGauge = TestRun.AtmosphericGauge;                           
        }

        private readonly ObservableAsPropertyHelper<decimal?> _absolutePressure;
        public decimal? AbsolutePressure => _absolutePressure.Value;

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

        public bool ShowAbsolute => TestRun.VerificationTest.Instrument.Transducer == TransducerType.Absolute;
        public bool ShowGaugeOnly => !ShowAbsolute;

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