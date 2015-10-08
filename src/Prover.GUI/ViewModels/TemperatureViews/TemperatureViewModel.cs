using System;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Prover.GUI.Events;

namespace Prover.GUI.ViewModels.TemperatureViews
{
    public class TemperatureViewModel : ReactiveScreen, IHandle<InstrumentUpdateEvent>
    {
        
        public TemperatureViewModel(IUnityContainer container)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);

            TestViews = new ObservableCollection<TemperatureTestViewModel>();
        }

        private IUnityContainer _container;
        public InstrumentManager InstrumentManager { get; set; }

        public LiveTemperatureReadViewModel LiveReadItem
        {
            get { return new LiveTemperatureReadViewModel(_container); }
        }

        public ObservableCollection<TemperatureTestViewModel> TestViews { get; set; }
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

        public void Handle(InstrumentUpdateEvent message)
        {
            InstrumentManager = message.InstrumentManager;

            InstrumentManager.Instrument.Temperature.Tests.ForEach(x =>
                TestViews.Add(new TemperatureTestViewModel(_container, InstrumentManager, x))
            );

            NotifyOfPropertyChange(() => InstrumentManager);
            NotifyOfPropertyChange(() => Temperature);
            NotifyOfPropertyChange(() => TestViews);
        }
    }
}
