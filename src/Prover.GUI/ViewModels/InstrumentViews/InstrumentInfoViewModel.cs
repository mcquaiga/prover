﻿using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.VerificationTests;
using Prover.Core.Models.Instruments;
using Prover.GUI.Events;
using Prover.Core.Extensions;

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

        public string BasePressure
        {
            get
            {
                return string.Format("{0} {1}", Instrument.EvcBasePressure(), Instrument.PressureUnits());
            }
        }


        public void Handle(InstrumentUpdateEvent message)
        {
            Instrument = message.InstrumentManager.Instrument;
            NotifyOfPropertyChange(()=>Instrument);
        }
    }
}