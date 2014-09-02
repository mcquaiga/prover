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
        public int TestIndex { get; set; }

        public string TestLevel
        {
            get { return "";  }
        }

        public TemperatureTestViewModel(IUnityContainer container, InstrumentManager instrumentManager, int testIndex)
        {
            _container = container;
            TestIndex = testIndex;
            InstrumentManager = instrumentManager;
        }

        public async void FetchTestItems()
        {
           await InstrumentManager.DownloadTemperatureTestItems();
        }

        public void Handle(InstrumentUpdateEvent message)
        {
            //Test = message;
        }
    }
}
