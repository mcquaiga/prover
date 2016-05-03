using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;
using NLog;
using Prover.Core;
using Prover.Core.Communication;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;
using Prover.GUI.Events;
using Prover.GUI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Prover.GUI.ViewModels.VerificationTestViews.PTVerificationViews
{
    public class PressureTestViewModel : BaseTestViewModel
    {
        private IUnityContainer _container;
        private readonly Logger _log = NLog.LogManager.GetCurrentClassLogger();

        public PressureTestViewModel(IUnityContainer container, PressureTest test)
        {
            _container = container;
            Test = test;
            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public bool ShowATMValues => (Test as PressureTest).VerificationTest.Instrument.GetTransducerType() != TransducerType.Absolute;
        public bool ShowATMGaugeInput => (Test as PressureTest).VerificationTest.Instrument.GetTransducerType() == TransducerType.Absolute;

        public decimal Gauge
        {
            get { return (Test as PressureTest).GasGauge.Value ; }
            set
            {
                (Test as PressureTest).GasGauge = value;
                _container.Resolve<IEventAggregator>().PublishOnUIThread(VerificationTestEvent.Raise());
            }
        }

        public decimal GasPressure => (Test as PressureTest).GasPressure.Value;

        public decimal? AtmosphericGauge
        {
            get { return (Test as PressureTest).AtmosphericGauge; }
            set
            {
                (Test as PressureTest).AtmosphericGauge = value;
                _container.Resolve<IEventAggregator>().PublishOnUIThread(VerificationTestEvent.Raise());
            }
        }

        public decimal? EvcGasPressure => (Test as PressureTest).ItemValues.EvcGasPressure();
        public decimal? EvcFactor => (Test as PressureTest).ItemValues.EvcPressureFactor();
        public decimal? EvcATMPressure => (Test as PressureTest).VerificationTest.Instrument.EvcAtmosphericPressure();

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
