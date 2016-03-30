using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Events;
using Prover.Core.VerificationTests;
using Prover.GUI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.GUI.ViewModels
{
    class LiveReadViewModel : ReactiveScreen, IHandle<LiveReadEvent>, IHandle<InstrumentUpdateEvent>
    {
        private readonly IUnityContainer _container;
        private TestManager _instrumentManager;

        public LiveReadViewModel(IUnityContainer container)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public decimal LiveReadValue { get; set; }

        public void Handle(LiveReadEvent message)
        {
            LiveReadValue = message.LiveReadValue;
            NotifyOfPropertyChange(() => LiveReadValue);
        }

        public void Handle(InstrumentUpdateEvent message)
        {
            _instrumentManager = message.InstrumentManager;
        }
    }
}
