using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.GUI.ViewModels
{
    public class CommunicationStatusViewModel : ReactiveScreen, IHandle<CommunicationStatusEvent>
    {
        public string StatusMessage { get; private set; }

        public CommunicationStatusViewModel(IUnityContainer container)
        {
            container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public void Handle(CommunicationStatusEvent message)
        {
            StatusMessage = message.Message;
        }
    }
}
