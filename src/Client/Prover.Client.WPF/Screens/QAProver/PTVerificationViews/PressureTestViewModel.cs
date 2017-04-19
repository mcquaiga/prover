namespace Prover.Client.WPF.Screens.QAProver.PTVerificationViews
{
    public class PressureTestViewModel : TestRunViewModelBase<Core.Models.Instruments.PressureTest>
    {
        public PressureTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            Core.Models.Instruments.PressureTest testRun) : base(screenManager, eventAggregator, testRun)
        {
        }

        public decimal? AtmosphericGauge
        {
            get { return TestRun.AtmosphericGauge; }
            set
            {
                TestRun.AtmosphericGauge = value;
                EventAggregator.PublishOnUIThread(VerificationTestEvent.Raise());
            }
        }

        public decimal? EvcATMPressure
            =>
                TestRun.VerificationTest.Instrument.Items.GetItem(
                    ItemCodes.Pressure.Atm).NumericValue;

        public decimal? EvcFactor
            => TestRun.Items.GetItem(ItemCodes.Pressure.Factor).NumericValue;

        public decimal? EvcGasPressure
            => TestRun.Items.GetItem(ItemCodes.Pressure.GasPressure).NumericValue
        ;

        public decimal GasPressure
        {
            get
            {
                if (TestRun.GasPressure != null) return TestRun.GasPressure.Value;
                return decimal.Zero;
            }
        }

        public decimal Gauge
        {
            get { return TestRun.GasGauge.Value; }
            set
            {
                TestRun.GasGauge = value;
                EventAggregator.PublishOnUIThread(VerificationTestEvent.Raise());
            }
        }

        public bool ShowATMGaugeInput
            =>
                (TransducerType)
                TestRun.VerificationTest.Instrument.Items.GetItem(
                    ItemCodes.Pressure.TransducerType).NumericValue == TransducerType.Absolute;

        public bool ShowATMValues
            =>
                (TransducerType)
                TestRun.VerificationTest.Instrument.Items.GetItem(
                    ItemCodes.Pressure.TransducerType).NumericValue != TransducerType.Absolute;

        public override void Handle(VerificationTestEvent message)
        {
            NotifyOfPropertyChange(() => TestRun);
            NotifyOfPropertyChange(() => TestRun.PercentError);
            NotifyOfPropertyChange(() => TestRun.HasPassed);
            NotifyOfPropertyChange(() => EvcGasPressure);
            NotifyOfPropertyChange(() => GasPressure);
            NotifyOfPropertyChange(() => EvcFactor);
            NotifyOfPropertyChange(() => EvcATMPressure);
            NotifyOfPropertyChange(() => Gauge);
            NotifyOfPropertyChange(() => AtmosphericGauge);
            NotifyOfPropertyChange(() => PercentColour);
        }
    }
}