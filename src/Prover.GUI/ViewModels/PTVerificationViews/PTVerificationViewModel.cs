﻿using System;
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

        public PTVerificationViewModel(IUnityContainer container, Instrument instrument, bool showLiveRead = true, bool showCommButtons = true) : this(container, showLiveRead, showCommButtons)
        {
            Instrument = instrument;

            TestViews = new ObservableCollection<PTVerificationSetViewModel>();

            Temperature.Tests.ForEach(x =>
                TestViews.Add(new PTVerificationSetViewModel(_container, InstrumentManager, x, showCommButtons)));
        }

        public bool ShowLiveRead
        {
            get; private set;
        }

        private IUnityContainer _container;
        public TestManager InstrumentManager { get; set; }
        public Instrument Instrument { get; set; }

        public ObservableCollection<PTVerificationSetViewModel> TestViews { get; set; }

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
                TestViews.Add(new PTVerificationSetViewModel(_container, InstrumentManager, x))
            );

            NotifyOfPropertyChange(() => InstrumentManager);
            NotifyOfPropertyChange(() => Temperature);
            NotifyOfPropertyChange(() => TestViews);
        }
    }
}
