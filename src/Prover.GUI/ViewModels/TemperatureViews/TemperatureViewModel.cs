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
        
        public TemperatureViewModel(IUnityContainer container, bool showLiveRead = true, bool showCommButtons = true)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
            ShowLiveRead = showLiveRead;
            TestViews = new ObservableCollection<TemperatureTestViewModel>();
        }
        
        public TemperatureViewModel(IUnityContainer container, Instrument instrument, bool showLiveRead = true, bool showCommButtons = true) : this(container, showLiveRead, showCommButtons)
        {
            Instrument = instrument;

            TestViews = new ObservableCollection<TemperatureTestViewModel>();

            Instrument.Temperature.Tests.ForEach(x =>
                TestViews.Add(new TemperatureTestViewModel(_container, x, showCommButtons)));
        }

        public TemperatureViewModel(IUnityContainer container, TestManager instrumentManager) : this(container, instrumentManager.Instrument, true, true)
        {

        }

        public bool ShowLiveRead
        {
            get; private set;
        }

        private IUnityContainer _container;
        public TestManager InstrumentManager { get; set; }
        public Instrument Instrument { get; set; }

        public LiveTemperatureReadViewModel LiveReadItem => ShowLiveRead ? new LiveTemperatureReadViewModel(_container) : null;

        public ObservableCollection<TemperatureTestViewModel> TestViews { get; set; }
        public Temperature Temperature
        {
            get
            {
                return Instrument?.Temperature;
            }
        }

        public void Handle(InstrumentUpdateEvent message)
        {
            InstrumentManager = message.InstrumentManager;

            InstrumentManager.Instrument.Temperature.Tests.ForEach(x =>
                TestViews.Add(new TemperatureTestViewModel(_container, x))
            );

            NotifyOfPropertyChange(() => InstrumentManager);
            NotifyOfPropertyChange(() => Temperature);
            NotifyOfPropertyChange(() => TestViews);
        }
    }
}
