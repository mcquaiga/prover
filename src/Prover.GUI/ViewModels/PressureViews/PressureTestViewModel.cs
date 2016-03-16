using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;
using NLog;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Prover.GUI.Events;
using Prover.GUI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Prover.GUI.ViewModels.PressureViews
{
    public class PressureTestViewModel : ReactiveScreen, IHandle<InstrumentUpdateEvent>, IHandle<VerificationTestEvent>
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

        public async Task LiveReadCommand()
        {
            var liveReadView = new LiveReadView
            {
                DataContext = new LiveReadViewModel(_container)
            };

            var reading = InstrumentManager.StartLiveRead(8);
            //show the dialog
            var result = await DialogHost.Show(liveReadView, "LiveReadDialog", ClosingEventHandler);

            //check the result...
            Console.WriteLine("Dialog was closed, the CommandParameter used to close it was: " + (result ?? "NULL"));
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            InstrumentManager.InstrumentCommunicator.Disconnect().Wait();
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

        public void Handle(VerificationTestEvent @event)
        {
            NotifyOfPropertyChange(() => Test);
            NotifyOfPropertyChange(() => PercentColour);
        }
    }
}
