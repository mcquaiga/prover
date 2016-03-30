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
using Prover.Core.VerificationTests;

namespace Prover.GUI.ViewModels.PTVerificationViews
{
    public class PTVerificationSetViewModel : ReactiveScreen, IHandle<InstrumentUpdateEvent>
    {
        private IUnityContainer _container;
        private Instrument _instrument;
        private Instrument.VerificationTest _verificationTest;

        public PTVerificationSetViewModel(IUnityContainer container, Instrument instrument, Instrument.VerificationTest verificationTest)
        {
            _container = container;
            VerificationTest = verificationTest;
            _instrument = instrument;

            CreateViews();

            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public PTVerificationSetViewModel(IUnityContainer container, TestManager instrumentManager, Instrument.VerificationTest verificationTest)
            :this(container, instrumentManager.Instrument, verificationTest)
        {
        }

        private void CreateViews()
        {
            if (_instrument.CorrectorType == CorrectorType.PressureTemperature)
            {
                TemperatureTestViewModel = new TemperatureTestViewModel(_container, VerificationTest.TemperatureTest);
                PressureTestViewModel = new PressureTestViewModel(_container, VerificationTest.PressureTest);
                SuperFactorTestViewModel = new SuperTestViewModel(_container, VerificationTest.SuperTest);
            }                

            if (_instrument.CorrectorType == CorrectorType.TemperatureOnly)
            {
                TemperatureTestViewModel = new TemperatureTestViewModel(_container, VerificationTest.TemperatureTest);
            }

            if (_instrument.CorrectorType == CorrectorType.PressureOnly)
            {
                PressureTestViewModel = new PressureTestViewModel(_container, VerificationTest.PressureTest);
            }
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
