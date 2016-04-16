using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Events;

namespace Prover.GUI.ViewModels
{
    public class CommunicationStatusViewModel : ReactiveScreen, IHandle<InstrumentConnectionEvent>
    {
        public string StatusMessage { get; private set; }

        public CommunicationStatusViewModel(IUnityContainer container)
        {
            container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public void Handle(InstrumentConnectionEvent message)
        {
            StatusMessage = message.ConnectionStatus.ToString();
        }
    }
}
