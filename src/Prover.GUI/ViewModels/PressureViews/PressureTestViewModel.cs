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
    public class PressureTestViewModel : ReactiveScreen, IHandle<VerificationTestEvent>
    {
        private IUnityContainer _container;
        private readonly Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private bool _isReportView;
        public PressureTest Test { get; set; }

        public bool ShowGaugeControl => !_isReportView;
        public bool ShowATMGaugeControl => Test.Pressure.TransducerType == TransducerType.Absolute;

        public PressureTestViewModel(IUnityContainer container, PressureTest test, bool isReportView = false)
        {
            _container = container;
            Test = test;
            _isReportView = isReportView;
            _container.Resolve<IEventAggregator>().Subscribe(this);
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

        public void LiveReadCommand()
        {
            var viewmodel = new LiveReadViewModel(_container, 8);
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
