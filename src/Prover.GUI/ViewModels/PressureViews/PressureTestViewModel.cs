using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using NLog;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Prover.GUI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Prover.GUI.ViewModels.PressureViews
{
    public class PressureTestViewModel : ReactiveScreen, IHandle<InstrumentUpdateEvent>
    {
        private IUnityContainer _container;
        private readonly Logger _log = NLog.LogManager.GetCurrentClassLogger();
        public TestManager InstrumentManager { get; set; }
        public bool ShowCommButton { get; }
        public PressureTest Test { get; set; }

        public bool ShowGaugeDecimalControl => ShowCommButton;
        public bool ShowGaugeText => !ShowCommButton;

        public PressureTest.PressureLevel TestLevel => Test.TestLevel;

        public PressureTestViewModel(IUnityContainer container, TestManager instrumentManager, PressureTest test, bool showCommButton = true)
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
                _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent(string.Format("Downloading {0} Pressure from instrument...", TestLevel.ToString())));
                await InstrumentManager.DownloadPressureTestItems(Test.TestLevel);
                Test = InstrumentManager.Instrument.Pressure.Tests.FirstOrDefault(x => x.TestLevel == TestLevel);
                _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Complete!"));
            }

            NotifyOfPropertyChange(() => Test);
            NotifyOfPropertyChange(() => PercentColour);
        }

        public decimal Gauge
        {
            get { return Test.GasGauge.Value ; }
            set
            {
                Test.GasGauge = value;
                NotifyOfPropertyChange(() => Test);
                NotifyOfPropertyChange(() => PercentColour);
            }
        }

        public decimal? AtmosphericGauge
        {
            get { return Test.AtmosphericGauge; }
            set
            {
                Test.AtmosphericGauge = value;
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
