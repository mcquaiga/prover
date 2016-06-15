using Caliburn.Micro;
using Microsoft.Practices.Unity;
using NLog;
using Prover.CommProtocol.Common.Items;
using Prover.Core;
using Prover.GUI.Common.Events;
using LogManager = NLog.LogManager;

namespace Prover.GUI.Screens.QAProver.VerificationTestViews.PTVerificationViews
{
    public class PressureTestViewModel : BaseTestViewModel
    {
        private readonly IUnityContainer _container;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public PressureTestViewModel(IUnityContainer container, Core.Models.Instruments.PressureTest test)
        {
            _container = container;
            Test = test;
            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public bool ShowATMValues
            =>
                (TransducerType)
                    (Test as Core.Models.Instruments.PressureTest).VerificationTest.Instrument.Items.GetItem(
                        ItemCodes.Pressure.TransducerType).NumericValue != TransducerType.Absolute;

        public bool ShowATMGaugeInput
            =>
                (TransducerType)
                    (Test as Core.Models.Instruments.PressureTest).VerificationTest.Instrument.Items.GetItem(
                        ItemCodes.Pressure.TransducerType).NumericValue == TransducerType.Absolute;

        public decimal Gauge
        {
            get { return (Test as Core.Models.Instruments.PressureTest).GasGauge.Value; }
            set
            {
                (Test as Core.Models.Instruments.PressureTest).GasGauge = value;
                _container.Resolve<IEventAggregator>().PublishOnUIThread(VerificationTestEvent.Raise());
            }
        }

        public decimal GasPressure => (Test as Core.Models.Instruments.PressureTest).GasPressure.Value;

        public decimal? AtmosphericGauge
        {
            get { return (Test as Core.Models.Instruments.PressureTest).AtmosphericGauge; }
            set
            {
                (Test as Core.Models.Instruments.PressureTest).AtmosphericGauge = value;
                _container.Resolve<IEventAggregator>().PublishOnUIThread(VerificationTestEvent.Raise());
            }
        }

        public decimal? EvcGasPressure
            => (Test as Core.Models.Instruments.PressureTest).Items.GetItem(ItemCodes.Pressure.GasPressure).NumericValue
            ;

        public decimal? EvcFactor
            => (Test as Core.Models.Instruments.PressureTest).Items.GetItem(ItemCodes.Pressure.Factor).NumericValue;

        public decimal? EvcATMPressure
            =>
                (Test as Core.Models.Instruments.PressureTest).VerificationTest.Instrument.Items.GetItem(
                    ItemCodes.Pressure.Atm).NumericValue;

        public override void Handle(VerificationTestEvent message)
        {
            NotifyOfPropertyChange(() => Test);
            NotifyOfPropertyChange(() => Test.PercentError);
            NotifyOfPropertyChange(() => Test.HasPassed);
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