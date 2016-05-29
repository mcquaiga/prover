using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.Core.Events;
using Prover.GUI.Events;
using Prover.GUI.Interfaces;
using Prover.GUI.ViewModels.SettingsViews;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Prover.GUI.ViewModels.Dialogs;

namespace Prover.GUI.ViewModels.Shell
{
    public class ShellViewModel : Conductor<object>.Collection.OneActive, IShell, 
        IHandle<ScreenChangeEvent>, 
        IHandle<NotificationEvent>,
        IHandle<ConnectionStatusEvent>
    {
        private readonly IUnityContainer _container;
        private System.Timers.Timer _timer;
        private string _applicationEventMessage;
        private MainMenuViewModel _mainMenu;
        private object _currentView;

        public ShellViewModel(IUnityContainer container)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
            _mainMenu = new MainMenuViewModel(_container);
            Notifications = new ToastNotificationViewModel(_container);
            ShowMainMenu();
        }

        private void ShowMainMenu()
        {
            _currentView = _mainMenu;
            ActivateItem(_mainMenu);
        }

        public async Task HomeButton()
        {
            await ScreenManager.Change(_container, _mainMenu);
        }

        public void SettingsButton()
        {
            ShowSettingsWindow();
        }

        private void ShowSettingsWindow()
        {
            ScreenManager.ShowDialog(_container, new SettingsViewModel(_container));
        }

        public ToastNotificationViewModel Notifications { get; set; }

        public string ApplicationEventMessage
        {
            get { return _applicationEventMessage; }
            set
            {
                _applicationEventMessage = value;
                NotifyOfPropertyChange(() => ApplicationEventMessage);
            }
        }

        public string Title => "EVC Prover";

        public ConnectionViewModel ConnectionDialogue { get; private set; }

        public void Handle(ScreenChangeEvent message)
        {
            if (_currentView.GetType().GetInterfaces().Contains(typeof(IDisposable)))
            {
                (_currentView as IDisposable).Dispose();
            }

            ActivateItem(message.ViewModel);
            _currentView = message.ViewModel;
        }

        public void Handle(NotificationEvent message)
        {
            _timer = new System.Timers.Timer(3000);
            ApplicationEventMessage = message.Message;
            _timer.Elapsed += OnApplicationEventMessage;
            _timer.Enabled = true;
        }

        private void OnApplicationEventMessage(object sender, ElapsedEventArgs e)
        {
            ApplicationEventMessage = "";
            _timer.Enabled = false;
        }

        public void Handle(ConnectionStatusEvent message)
        {
            
        }
    }
}
