using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Prover.GUI.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.GUI.ViewModels.PressureViews
{
    public class PressureViewModel : ReactiveScreen, IHandle<InstrumentUpdateEvent>
    {
        private IUnityContainer _container;

        public PressureViewModel(IUnityContainer container,  bool showLiveRead = true, bool showCommButtons = true)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
            ShowLiveRead = showLiveRead;
            TestViews = new ObservableCollection<PressureTestViewModel>();
        }

        public PressureViewModel(IUnityContainer container, Instrument instrument, bool showLiveRead = true, bool showCommButtons = true) : this(container, showLiveRead, showCommButtons)
        {
            Instrument = instrument;

            Instrument.Pressure.Tests.ForEach(x =>
                TestViews.Add(new PressureTestViewModel(_container, x, showCommButtons)));
        }

        public PressureViewModel(IUnityContainer container, TestManager testManager, bool showLiveRead = true, bool showCommButtons = true) 
            : this(container, testManager.Instrument, showLiveRead, showCommButtons)
        {
            TestManager = testManager;

            TestManager.Instrument.Pressure.Tests.ForEach(x =>
                TestViews.Add(new PressureTestViewModel(_container, x))
            );
        }

        public Instrument Instrument { get; set; }
        public TestManager TestManager { get; private set; }
        public bool ShowLiveRead { get; private set; }

        public ObservableCollection<PressureTestViewModel> TestViews { get; private set; } = new ObservableCollection<PressureTestViewModel>();

        public Pressure Pressure
        {
            get
            {
                return Instrument?.Pressure;
            }
        }

        public void Handle(InstrumentUpdateEvent message)
        {
            NotifyOfPropertyChange(() => Pressure);
            NotifyOfPropertyChange(() => TestViews);
        }
    }
}
