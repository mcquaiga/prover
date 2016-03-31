using System;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Prover.GUI.Events;
using Prover.Core.VerificationTests;
using System.Linq;

namespace Prover.GUI.ViewModels.PTVerificationViews
{
    public class PTVerificationViewModel : ReactiveScreen, IHandle<InstrumentUpdateEvent>
    {
        public PTVerificationViewModel(IUnityContainer container, bool isReportView = false)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
            IsReportView = isReportView;
            TestViews = new ObservableCollection<PTVerificationSetViewModel>();
        }

        public PTVerificationViewModel(IUnityContainer container, Instrument instrument, bool isReportView = false) : this(container, isReportView)
        {
            Instrument = instrument;
            instrument.VerificationTests.OrderBy(v => v.TestNumber).ForEach(x =>
                TestViews.Add(new PTVerificationSetViewModel(_container, instrument, x, isReportView)));
        }

        public PTVerificationViewModel(IUnityContainer container, TestManager instrumentTestManager, bool isReportView = false) : this(container, isReportView)
        {
            InstrumentManager = instrumentTestManager;
            Instrument = InstrumentManager.Instrument;

            InstrumentManager.Instrument.VerificationTests.ForEach(x =>
                TestViews.Add(new PTVerificationSetViewModel(_container, InstrumentManager, x))
            );
        }

        public bool IsReportView
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
