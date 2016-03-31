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
        private bool _isReportView;

        public TemperatureTest Test { get; private set; }

        public bool ShowGaugeInputTextBox => !_isReportView;

        public TemperatureTestViewModel(IUnityContainer container, TemperatureTest test, bool isReportView = false)
        {
            _container = container;
            Test = test;
            _isReportView = isReportView;
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
