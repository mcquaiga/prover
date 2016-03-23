using Caliburn.Micro.ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Caliburn.Micro;
using Prover.GUI.Events;
using Prover.GUI.ViewModels.TemperatureViews;
using Prover.GUI.ViewModels.PressureViews;
using Prover.GUI.ViewModels.SuperFactorViews;

namespace Prover.GUI.ViewModels.PTVerificationViews
{
    public class PTVerificationSetViewModel : ReactiveScreen, IHandle<InstrumentUpdateEvent>
    {
        private bool showCommButtons;
        private IUnityContainer _container;
        private Instrument instrument;
        private Instrument.VerificationTest x;

        public PTVerificationSetViewModel(IUnityContainer container, TestManager instrumentManager, Instrument.VerificationTest verificationTest)
        {
            _container = container;
            InstrumentManager = instrumentManager;
            VerificationTest = verificationTest;
            TemperatureTestViewModel = new TemperatureTestViewModel(container, instrumentManager, verificationTest.TemperatureTest);
            PressureTestViewModel = new PressureTestViewModel(container, instrumentManager, verificationTest.PressureTest);
            SuperFactorTestViewModel = new SuperTestViewModel(container, instrumentManager, verificationTest.SuperTest);

            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public PTVerificationSetViewModel(IUnityContainer _container, Instrument instrument, Instrument.VerificationTest x)
        {
            this._container = _container;
            this.instrument = instrument;
            this.x = x;
        }

        public string Level
        {
            get
            {
                return string.Format("Level {0}", VerificationTest.TestNumber + 1);
            }
        }

        public TemperatureTestViewModel TemperatureTestViewModel { get; private set; }
        public PressureTestViewModel PressureTestViewModel { get; private set; }
        public SuperTestViewModel SuperFactorTestViewModel { get; private set; }

        public TestManager InstrumentManager { get; private set; }
        public Instrument.VerificationTest VerificationTest { get; private set; }

        public async Task DownloadItems()
        {
            await InstrumentManager.DownloadVerificationTestItems(VerificationTest.TestNumber);
            _container.Resolve<IEventAggregator>().PublishOnUIThread(new VerificationTestEvent(InstrumentManager));
        }

        public void Handle(InstrumentUpdateEvent message)
        {
            InstrumentManager = message.InstrumentManager;
        }

    }
}
