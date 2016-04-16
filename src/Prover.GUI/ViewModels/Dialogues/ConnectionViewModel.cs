using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Prover.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.GUI.ViewModels.Dialogues
{
    public class ConnectionViewModel : ReactiveScreen, IHandle<InstrumentConnectionEvent>
    {
        public string StatusText { get; private set; }
        public string AttempText { get; private set; }

        public ConnectionViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);
        }

        public void Handle(InstrumentConnectionEvent message)
        {
            StatusText = message.ConnectionStatus.ToString();

            if (message.ConnectionStatus == InstrumentConnectionEvent.Status.Connecting)
            {
                StatusText = StatusText + "...";
                AttempText = string.Format("Attempt {0} of {1}", message.AttemptCount, message.MaxAttempts);
            }
            else
            {
                StatusText = StatusText + "!";
                AttempText = "";
            }


            NotifyOfPropertyChange(() => StatusText);
            NotifyOfPropertyChange(() => AttempText);

            if (message.ConnectionStatus == InstrumentConnectionEvent.Status.Connected)
            {
                System.Threading.Thread.Sleep(500);
                this.TryClose();
            }
        }
    }
}
