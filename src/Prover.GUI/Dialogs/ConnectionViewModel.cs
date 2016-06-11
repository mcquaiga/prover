using System;
using System.Dynamic;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Events;
using Prover.GUI.Screens;

namespace Prover.GUI.Dialogs
{
    public class ConnectionViewModel : ReactiveScreen, IHandle<ConnectionStatusEvent>, IWindowSettings
    {
        private readonly IUnityContainer _container;

        public ConnectionViewModel(IUnityContainer container)
        {
            _container = container;
            container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public string StatusText { get; private set; }
        public string AttemptText { get; private set; }

        public void Handle(ConnectionStatusEvent message)
        {
            StatusText = message.ConnectionStatus.ToString();

            if (message.ConnectionStatus == ConnectionStatusEvent.Status.Connecting)
            {
                StatusText = StatusText + "...";
                AttemptText = $"Attempt {message.AttemptCount} of {message.MaxAttempts}";
            }
            else
            {
                StatusText = StatusText + "!";
                AttemptText = "";
            }


            NotifyOfPropertyChange(() => StatusText);
            NotifyOfPropertyChange(() => AttemptText);
        }

        public dynamic WindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.MinWidth = 450;
                settings.Title = "Connecting...";
                return settings;
            }
        }
    }

    public class CommuncationCommand : ICommand
    {
        private readonly Action<object> _execute;

        public CommuncationCommand(Action<object> execute)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));

            _execute = execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public static void InstrumentCommand(IUnityContainer container, Action<object> execute)
        {
            //var dialog = new ConnectionViewModel(container.Resolve<IEventAggregator>(), execute);
            //ScreenManager.ShowDialog(container, dialog);
        }
    }
}