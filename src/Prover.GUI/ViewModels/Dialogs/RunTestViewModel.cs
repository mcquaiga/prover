using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Events;
using Prover.GUI.ViewModels.VerificationTestViews.PTVerificationViews;
using Action = System.Action;

namespace Prover.GUI.ViewModels.Dialogs
{
    public class RunTestViewModel : ReactiveScreen, IHandle<TestRunProgressEvent>
    {
        private IUnityContainer _container;

        public RunTestViewModel(IUnityContainer container, Action runAction)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
            
            Task.Run(() => runAction());
        }

        public ReactiveScreen ActiveViewModel { get; set; }

        public void Handle(TestRunProgressEvent message)
        {
            if (message.TestStep == TestRunProgressEvent.TestRunStep.InstrumentCommunication)
            {
                ActiveViewModel = new CommunicationStatusViewModel(_container);
            }

            if (message.TestStep == TestRunProgressEvent.TestRunStep.LiveReading)
            {
                ActiveViewModel = new LiveReadViewModel(_container);
            }

            if (message.TestStep == TestRunProgressEvent.TestRunStep.RunningVolumeTest)
            {
                ActiveViewModel = new VolumeTestViewModel(_container);
            }
        }
    }
}
