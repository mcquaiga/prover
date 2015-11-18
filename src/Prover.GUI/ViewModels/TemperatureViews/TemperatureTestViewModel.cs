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
    public class TemperatureTestViewModel : ReactiveScreen, IHandle<InstrumentUpdateEvent>
    {
        private IUnityContainer _container;
        private readonly Logger _log = NLog.LogManager.GetCurrentClassLogger();
        public InstrumentManager InstrumentManager { get; set; }
        public bool ShowCommButton { get; }
        public TemperatureTest Test { get; set; }

        public bool ShowGaugeDecimalControl => ShowCommButton;
        public bool ShowGaugeText => !ShowCommButton;
        public TemperatureTest.Level TestLevel
        {
            get { return Test.TestLevel; }
        }

        public TemperatureTestViewModel(IUnityContainer container, InstrumentManager instrumentManager, TemperatureTest test, bool showCommButton = true)
        {
            _container = container;
            Test = test;
            InstrumentManager = instrumentManager;
            ShowCommButton = showCommButton;
            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public async void FetchTestItems()
        {
            if (InstrumentManager != null)
            {
                try
                {
                    _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent(string.Format("Downloading {0} Temperature from instrument...", TestLevel.ToString())));
                    await InstrumentManager.DownloadTemperatureTestItems(Test.TestLevel);
                    Test = InstrumentManager.Instrument.Temperature.Tests.FirstOrDefault(x => x.TestLevel == TestLevel);
                    _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Complete!"));
                }
                catch (Exception ex)
                {
                    _container.Resolve<EventAggregator>().PublishOnBackgroundThread("Framing error! Try again.");
                }
            }

            NotifyOfPropertyChange(() => Test);
            NotifyOfPropertyChange(() => PercentColour);
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
    }
}
