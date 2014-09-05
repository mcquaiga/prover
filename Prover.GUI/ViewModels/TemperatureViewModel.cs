using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Prover.GUI.Events;

namespace Prover.GUI.ViewModels
{
    public class TemperatureViewModel : ReactiveScreen, IHandle<InstrumentUpdateEvent>
    {
        private IUnityContainer _container;
        public InstrumentManager InstrumentManager { get; set; }
        public ICollection<TemperatureTestViewModel> TestViewModels { get; set; } 

        public TemperatureViewModel(IUnityContainer container)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);

            TestViewModels = new Collection<TemperatureTestViewModel>();
            InstrumentManager.Instrument.Temperature.Tests.ForEach(
                x => TestViewModels.Add(new TemperatureTestViewModel(_container, x))
                );
        }

        public void Handle(InstrumentUpdateEvent message)
        {
            InstrumentManager = message.InstrumentManager;
        }
    }
}
