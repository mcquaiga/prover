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

namespace Prover.GUI.ViewModels.VerificationTestViews.PTVerificationViews
{
    public class TemperatureTestViewModel : ReactiveScreen, IHandle<VerificationTestEvent>
    {
        private IUnityContainer _container;
        private readonly Logger _log = NLog.LogManager.GetCurrentClassLogger();

        public TemperatureTest Test { get; private set; }

        public TemperatureTestViewModel(IUnityContainer container, TemperatureTest test)
        {
            _container = container;
            Test = test;
            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public double Gauge
        {
            get { return Test.Gauge; }
            set
            {
                Test.Gauge = value;
                _container.Resolve<IEventAggregator>().PublishOnUIThread(VerificationTestEvent.Raise());
            }
        }

        public void StartLiveReadCommand()
        {
            var viewmodel = new LiveReadViewModel(_container, 26);
            ScreenManager.ShowDialog(_container, viewmodel);
        }

        public Brush PercentColour => Test.HasPassed ? Brushes.Green : Brushes.Red;

        public void Handle(VerificationTestEvent @event)
        {
            NotifyOfPropertyChange(() => Test);
            NotifyOfPropertyChange(() => PercentColour);
        }
    }
}
