using System.Linq;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Prover.GUI.Events;
using System.Windows.Media;
using System;
using NLog;
using Prover.Core.Extensions;

namespace Prover.GUI.ViewModels.VerificationTestViews.PTVerificationViews
{
    public class TemperatureTestViewModel : BaseTestViewModel
    {
        private IUnityContainer _container;
        private readonly Logger _log = NLog.LogManager.GetCurrentClassLogger();

        public TemperatureTestViewModel(IUnityContainer container, TemperatureTest test)
        {
            _container = container;
            Test = test;
            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public double Gauge
        {
            get { return (Test as TemperatureTest).Gauge; }
            set
            {
                (Test as TemperatureTest).Gauge = value;
                _container.Resolve<IEventAggregator>().PublishOnUIThread(VerificationTestEvent.Raise());
            }
        }

        public decimal? EvcReading => (Test as TemperatureTest).ItemValues.EvcTemperatureReading();
        public decimal? EvcFactor => (Test as TemperatureTest).ItemValues.EvcTemperatureFactor();

        public override void Handle(VerificationTestEvent @event)
        {
            NotifyOfPropertyChange(() => Test);
            NotifyOfPropertyChange(() => PercentError);
            NotifyOfPropertyChange(() => Gauge);
            NotifyOfPropertyChange(() => EvcReading);
            NotifyOfPropertyChange(() => EvcFactor);
            NotifyOfPropertyChange(() => PercentColour);
        }
    }
}
