using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.VerificationTests;
using Prover.Core.Models.Instruments;
using Prover.GUI.Events;

namespace Prover.GUI.ViewModels.InstrumentViews
{
    public class InstrumentInfoViewModel : ReactiveScreen, IHandle<InstrumentUpdateEvent>
    {
        private IUnityContainer _container;

        public InstrumentInfoViewModel(IUnityContainer container)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public InstrumentInfoViewModel(IUnityContainer container, Instrument instrument) : this(container)
        {
            Instrument = instrument;
        }

        public Instrument Instrument { get; set; }
        public void Handle(InstrumentUpdateEvent message)
        {
            Instrument = message.InstrumentManager.Instrument;
            NotifyOfPropertyChange(()=>Instrument);
        }
    }
}
