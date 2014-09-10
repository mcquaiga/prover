using System.Collections.ObjectModel;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Prover.GUI.Events;
using Prover.GUI.ViewModels;

namespace Prover.GUI.ViewModels
{
    public class TemperatureViewModel : ReactiveScreen, IHandle<InstrumentUpdateEvent>
    {
        private IUnityContainer _container;
        public InstrumentManager InstrumentManager { get; set; }
        public ObservableCollection<TemperatureTestViewModel> TestViews { get; set; }

        public TemperatureViewModel(IUnityContainer container)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);

            TestViews = new ObservableCollection<TemperatureTestViewModel>();
        }

        public void Handle(InstrumentUpdateEvent message)
        {
            InstrumentManager = message.InstrumentManager;
            InstrumentManager.Instrument.Temperature.Tests.ForEach(
                x => TestViews.Add(new TemperatureTestViewModel(_container,InstrumentManager, x))
                );
            NotifyOfPropertyChange(() => InstrumentManager);
            NotifyOfPropertyChange(() => Temperature);
            NotifyOfPropertyChange(() => TestViews);

        }

        public Temperature Temperature
        {
            get
            {
                if (InstrumentManager != null)
                {
                    return InstrumentManager.Instrument.Temperature;
                }
                return null;
            }
        }

    }
}
