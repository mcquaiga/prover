using System.Linq;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Prover.GUI.Events;

namespace Prover.GUI.ViewModels
{
    public class TemperatureTestViewModel : ReactiveScreen, IHandle<InstrumentUpdateEvent>
    {
        private IUnityContainer _container;
        public InstrumentManager InstrumentManager { get; set; }

        public TemperatureTest Test { get; set; }

        public TemperatureTest.Level TestLevel
        {
            get { return Test.TestLevel; }
        }

        public TemperatureTestViewModel(IUnityContainer container, InstrumentManager instrumentManager, TemperatureTest test)
        {
            _container = container;
            Test = test;
            InstrumentManager = instrumentManager;
            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public async void FetchTestItems()
        {
            if (InstrumentManager != null)
            {
                await InstrumentManager.DownloadTemperatureTestItems(Test.TestLevel);
                Test = InstrumentManager.Instrument.Temperature.Tests.FirstOrDefault(x => x.TestLevel == TestLevel);
            }
            NotifyOfPropertyChange(() => Test);
        }

        public double Gauge
        {
            get { return Test.Gauge; }
            set
            {
                Test.Gauge = value;
                NotifyOfPropertyChange(() => Test);
            }
        }

        public void Handle(InstrumentUpdateEvent message)
        {
            InstrumentManager = message.InstrumentManager;
        }
    }
}
