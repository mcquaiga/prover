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

namespace Prover.GUI.ViewModels.SuperFactorViews
{
    public class SuperTestViewModel : ReactiveScreen, IHandle<InstrumentUpdateEvent>
    {
        private IUnityContainer _container;
        private readonly Logger _log = NLog.LogManager.GetCurrentClassLogger();
        public TestManager InstrumentManager { get; set; }
        public SuperFactor Test { get; set; }

        public SuperTestViewModel(IUnityContainer container, TestManager instrumentManager, SuperFactor test)
        {
            _container = container;
            Test = test;
            InstrumentManager = instrumentManager;
            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public Brush PercentColour => Test.HasPassed ? Brushes.Green : Brushes.Red;

        public void Handle(InstrumentUpdateEvent message)
        {
            InstrumentManager = message.InstrumentManager;
        }
    }
}
