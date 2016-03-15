using Caliburn.Micro.ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Caliburn.Micro;
using Prover.GUI.Events;

namespace Prover.GUI.ViewModels.PTVerificationViews
{
    public class PTVerificationSetViewModel : ReactiveScreen, IHandle<InstrumentUpdateEvent>
    {
        private bool showCommButtons;
        private IUnityContainer _container;

        public PTVerificationSetViewModel(IUnityContainer container, TestManager instrumentManager, TemperatureTest temperatureTest, PressureTest pressureTest, SuperFactor superFactorTest)
        {
            _container = container;
            InstrumentManager = instrumentManager;
            TemperatureTest = temperatureTest;
            PressureTest = pressureTest;
            SuperFactorTest = superFactorTest;
            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public TemperatureTest TemperatureTest { get; private set; }
        public PressureTest PressureTest { get; private set; }
        public SuperFactor SuperFactorTest { get; private set; }
        public TestManager InstrumentManager { get; private set; }

        public void Handle(InstrumentUpdateEvent message)
        {
            InstrumentManager = message.InstrumentManager;
        }

    }
}
