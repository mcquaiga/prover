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
        
        public TemperatureViewModel(IUnityContainer container, bool showLiveRead = true)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
            ShowLiveRead = showLiveRead;
            TestViews = new ObservableCollection<TemperatureTestViewModel>();
        }
        
        public TemperatureViewModel(IUnityContainer container, Instrument instrument, bool showLiveRead = true) : this(container, showLiveRead)
        {
            Instrument = instrument;

            TestViews = new ObservableCollection<TemperatureTestViewModel>();

            Temperature.Tests.ForEach(x =>
                TestViews.Add(new TemperatureTestViewModel(_container, InstrumentManager, x)));
        }

        public bool ShowLiveRead
        {
            get; private set;
        }

        private IUnityContainer _container;
        public InstrumentManager InstrumentManager { get; set; }
        public Instrument Instrument { get; set; }

        public LiveTemperatureReadViewModel LiveReadItem => ShowLiveRead ? new LiveTemperatureReadViewModel(_container) : null;

        public ObservableCollection<TemperatureTestViewModel> TestViews { get; set; }
        public Temperature Temperature
        {
            get
            {
                if (InstrumentManager != null)
                {
                    return InstrumentManager.Instrument.Temperature;
                }
                
                return Instrument?.Temperature;
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
