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
    public class TemperatureTestViewModel : ReactiveScreen, IHandle<InstrumentUpdateEvent>, IHandle<VerificationTestEvent>
    {
        private IUnityContainer _container;
        private readonly Logger _log = NLog.LogManager.GetCurrentClassLogger();
        public TestManager InstrumentManager { get; set; }
        public bool ShowCommButton { get; }
        public TemperatureTest Test { get; set; }

        public bool ShowGaugeDecimalControl => ShowCommButton;
        public bool ShowGaugeText => !ShowCommButton;

        public TemperatureTest.Level TestLevel => Test.TestLevel;

        public TemperatureTestViewModel(IUnityContainer container, TestManager instrumentManager, TemperatureTest test, bool showCommButton = true)
        {
            _container = container;
            Test = test;
            InstrumentManager = instrumentManager;
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

        public Brush PercentColour => Test.HasPassed ? Brushes.Green : Brushes.Red;

        public void Handle(InstrumentUpdateEvent message)
        {
            InstrumentManager = message.InstrumentManager;
        }

        public void Handle(VerificationTestEvent @event)
        {
            NotifyOfPropertyChange(() => Test);
            NotifyOfPropertyChange(() => PercentColour);
        }
    }
}
