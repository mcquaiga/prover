using System;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Prover.GUI.Events;

namespace Prover.GUI.ViewModels.PTVerificationViews
{
    public class PTVerificationViewModel : ReactiveScreen, IHandle<InstrumentUpdateEvent>
    {
        public PTVerificationViewModel(IUnityContainer container, bool showLiveRead = true, bool showCommButtons = true)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
            ShowLiveRead = showLiveRead;
            TestViews = new ObservableCollection<PTVerificationSetViewModel>();
        }

        public PTVerificationViewModel(IUnityContainer container, Instrument instrument, bool showLiveRead = false, bool showCommButtons = false) : this(container, showLiveRead, showCommButtons)
        {
            Instrument = instrument;

            TestViews = new ObservableCollection<PTVerificationSetViewModel>();

            instrument.VerificationTests.ForEach(x =>
                TestViews.Add(new PTVerificationSetViewModel(_container, instrument, x)));
        }

        public PTVerificationViewModel(IUnityContainer container, TestManager instrumentTestManager, bool showLiveRead = true, bool showCommButtons = true) : this(container, showLiveRead, showCommButtons)
        {
            InstrumentManager = instrumentTestManager;
            Instrument = InstrumentManager.Instrument;

            InstrumentManager.Instrument.VerificationTests.ForEach(x =>
                TestViews.Add(new PTVerificationSetViewModel(_container, InstrumentManager, x))
            );
        }

        public bool ShowLiveRead
        {
            get; private set;
        }

        private IUnityContainer _container;

        public TestManager InstrumentManager { get; set; }
        public Instrument Instrument { get; set; }

        public ObservableCollection<PTVerificationSetViewModel> TestViews { get; set; }

        public void Handle(InstrumentUpdateEvent message)
        {
            InstrumentManager = message.InstrumentManager;

            InstrumentManager.Instrument.VerificationTests.ForEach(x =>
                TestViews.Add(new PTVerificationSetViewModel(_container, InstrumentManager, x))
            );

            NotifyOfPropertyChange(() => InstrumentManager);
            NotifyOfPropertyChange(() => TestViews);
        }
    }
}
