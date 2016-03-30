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

namespace Prover.GUI.ViewModels.TemperatureViews
{
    public class TemperatureTestViewModel : ReactiveScreen, IHandle<VerificationTestEvent>
    {
        private IUnityContainer _container;
        private readonly Logger _log = NLog.LogManager.GetCurrentClassLogger();
        public bool ShowCommButton { get; }
        public TemperatureTest Test { get; set; }

        public bool ShowGaugeDecimalControl => ShowCommButton;
        public bool ShowGaugeText => !ShowCommButton;

        public TemperatureTestViewModel(IUnityContainer container, TemperatureTest test, bool showCommButton = true)
        {
            _container = container;
            Test = test;
            ShowCommButton = showCommButton;
            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public double Gauge
        {
            get { return Test.Gauge; }
            set
            {
                Test.Gauge = value;
                NotifyOfPropertyChange(() => Test);
                NotifyOfPropertyChange(() => PercentColour);
            }
        }

        public void StartLiveReadCommand()
        {

        }

        public Brush PercentColour => Test.HasPassed ? Brushes.Green : Brushes.Red;

        public void Handle(VerificationTestEvent @event)
        {
            NotifyOfPropertyChange(() => Test);
            NotifyOfPropertyChange(() => PercentColour);
        }
    }
}
