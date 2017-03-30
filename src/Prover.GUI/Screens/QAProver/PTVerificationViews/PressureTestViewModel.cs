using Caliburn.Micro;
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
            GasGauge = TestRun.GasGauge ?? 0;
            AtmosphericGauge = TestRun.AtmosphericGauge ?? 0;

            this.WhenAnyValue(x => x.TestRun.AtmosphericGauge);
            this.WhenAnyValue(x => x.TestRun.GasGauge);
        }

        public bool ShowAtmValues => 
            TestRun.VerificationTest.Instrument.Transducer == TransducerType.Absolute;

        private decimal _gasGauge;
        public decimal GasGauge
        {
            get { return _gasGauge; }
            set { this.RaiseAndSetIfChanged(ref _gasGauge, value); }
        }

        public decimal GasGaugeCal
        {
            get
            {
                if (TestRun.VerificationTest.Instrument.Transducer == TransducerType.Absolute)
                    return (TestRun.GasGauge ?? 0) - (AtmosphericGauge ?? 0);

                return TestRun.GasGauge ?? 0;
            }
        }

        private decimal? _atmosphericGauge;
        public decimal? AtmosphericGauge
        {
            get { return _atmosphericGauge; }
            set
            {
                this.RaiseAndSetIfChanged(ref _atmosphericGauge, value);
                TestRun.AtmosphericGauge = value;
                RaisePropertyChangeEvents();
                EventAggregator.PublishOnUIThread(VerificationTestEvent.Raise());
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


        private void RaisePropertyChangeEvents()
        {
            NotifyOfPropertyChange(() => TestRun);
            NotifyOfPropertyChange(() => TestRun.PercentError);
            NotifyOfPropertyChange(() => TestRun.HasPassed);
            NotifyOfPropertyChange(() => EvcGasPressure);
            NotifyOfPropertyChange(() => GasGauge);
            NotifyOfPropertyChange(() => EvcFactor);
            NotifyOfPropertyChange(() => EvcAtmPressure);
            NotifyOfPropertyChange(() => AtmosphericGauge);
            NotifyOfPropertyChange(() => PercentColour);
        }

        public override void Handle(VerificationTestEvent message)
        {
            RaisePropertyChangeEvents();
        }
    }
}