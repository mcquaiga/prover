using Caliburn.Micro;
using Microsoft.Practices.Unity;
using NLog;
using Prover.CommProtocol.Common.Items;
using Prover.GUI.Common.Events;
using LogManager = NLog.LogManager;

namespace Prover.GUI.Screens.QAProver.VerificationTestViews.PTVerificationViews
{
    public class TemperatureTestViewModel : BaseTestViewModel
    {
        private readonly IUnityContainer _container;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public TemperatureTestViewModel(IUnityContainer container, Core.Models.Instruments.TemperatureTest test)
        {
            _container = container;
            Test = test;
            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public double Gauge
        {
            get { return (Test as Core.Models.Instruments.TemperatureTest).Gauge; }
            set
            {
                (Test as Core.Models.Instruments.TemperatureTest).Gauge = value;
                _container.Resolve<IEventAggregator>().PublishOnUIThread(VerificationTestEvent.Raise());
            }
        }

        public decimal? EvcReading => (Test as Core.Models.Instruments.TemperatureTest).Items?.GetItem(26).NumericValue;
        public decimal? EvcFactor => (Test as Core.Models.Instruments.TemperatureTest).Items?.GetItem(45).NumericValue;

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