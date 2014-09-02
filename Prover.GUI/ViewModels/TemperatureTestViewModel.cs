using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Prover.GUI.Events;

namespace Prover.GUI.ViewModels
{
    public class TemperatureTestViewModel: ReactiveScreen, IHandle<InstrumentUpdateEvent>
    {
        private IUnityContainer _container;
        public InstrumentManager InstrumentManager { get; set; }
        public TemperatureTest Test { get; set; }

        public string TestLevel
        {
            get { return "";  }
        }

        public TemperatureTestViewModel(IUnityContainer container, TemperatureTest test)
        {
            _container = container;
            Test = test;
        }

        public async void FetchTestItems()
        {
            if (InstrumentManager != null)
                await InstrumentManager.DownloadTemperatureTestItems(Test.TestLevel);
        }

        public void Handle(InstrumentUpdateEvent message)
        {
            InstrumentManager = message.InstrumentManager;
        }
    }
}
