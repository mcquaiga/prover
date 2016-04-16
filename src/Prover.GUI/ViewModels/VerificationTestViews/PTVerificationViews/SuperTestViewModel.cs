using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using NLog;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Prover.Core.VerificationTests;
using Prover.GUI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Prover.GUI.ViewModels.VerificationTestViews.PTVerificationViews
{
    public class SuperTestViewModel : ReactiveScreen, IHandle<VerificationTestEvent>
    {
        private IUnityContainer _container;
        private readonly Logger _log = NLog.LogManager.GetCurrentClassLogger();
        public RotaryTestManager InstrumentManager { get; set; }
        public SuperFactor Test { get; set; }

        public SuperTestViewModel(IUnityContainer container, SuperFactor test)
        {
            _container = container;
            Test = test;
            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public Brush PercentColour => Test.HasPassed ? Brushes.Green : Brushes.Red;

        public void Handle(VerificationTestEvent message)
        {
            NotifyOfPropertyChange(() => Test);
        }
    }
}
