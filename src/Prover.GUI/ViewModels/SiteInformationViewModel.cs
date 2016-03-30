using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Prover.GUI.Events;
using Prover.Core.VerificationTests;

namespace Prover.GUI.ViewModels
{
    public class SiteInformationViewModel : ReactiveScreen, IHandle<InstrumentUpdateEvent>
    {
        private IUnityContainer _container;
        public SiteInformationViewModel(IUnityContainer container)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public SiteInformationViewModel(IUnityContainer container, TestManager instrumentTestManager) : this(container)
        {
            Instrument = instrumentTestManager.Instrument;
        }

        public SiteInformationViewModel(IUnityContainer container, Instrument instrument) : this(container)
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
