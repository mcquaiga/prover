using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Prover.GUI.ViewModels.Dialogues
{
    public class CommuncationCommand : ICommand
    {
        private readonly Action<object> _execute;

        public event EventHandler CanExecuteChanged;

        public static void InstrumentCommand(IUnityContainer container, Action<object> execute)
        {
            var dialog = new ConnectionViewModel(container.Resolve<IEventAggregator>(), execute);
            ScreenManager.ShowDialog(container, dialog);
        }

        public CommuncationCommand(Action<object> execute)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));

            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }

    public class ConnectionViewModel : ReactiveScreen, IHandle<InstrumentConnectionEvent>
    {
        private CommuncationCommand _command;

        public string StatusText { get; private set; }
        public string AttempText { get; private set; }

        public ConnectionViewModel(IEventAggregator eventAggregator, Action<object> execute)
        {
            eventAggregator.Subscribe(this);
            _command = new CommuncationCommand(execute);
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
