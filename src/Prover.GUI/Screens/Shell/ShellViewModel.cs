using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Events;
using Prover.Core.Login;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;
using Prover.GUI.Common.Interfaces;
using Prover.GUI.Dialogs;
using Prover.GUI.Screens.Settings;

namespace Prover.GUI.Screens.Shell
{
    public class ShellViewModel : Conductor<object>.Collection.OneActive, IShell,
        IHandle<ScreenChangeEvent>,
        IHandle<NotificationEvent>,
        IHandle<ConnectionStatusEvent>
    {
        private readonly IUnityContainer _container;
        private readonly MainMenuViewModel _mainMenu;
        private string _applicationEventMessage;
        private object _currentView;
        private Timer _timer;

        public ShellViewModel(IUnityContainer container)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
            _mainMenu = new MainMenuViewModel(_container);
            ShowMainMenu();
        }

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

        public void Handle(ConnectionStatusEvent message)
        {
        }

        public void Handle(NotificationEvent message)
        {
            _timer = new Timer(3000);
            ApplicationEventMessage = message.Message;
            _timer.Elapsed += OnApplicationEventMessage;
            _timer.Enabled = true;
        }

        public void Handle(ScreenChangeEvent message)
        {
            if (_currentView.GetType().GetInterfaces().Contains(typeof(IDisposable)))
            {
                (_currentView as IDisposable).Dispose();
            }
            (_currentView as ReactiveScreen).TryClose();
            ActivateItem(message.ViewModel);
            _currentView = message.ViewModel;
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

        public async Task LoginButton()
        {
            await _container.Resolve<ILoginService>().Login();
        }

        private void ShowSettingsWindow()
        {
            ScreenManager.ShowDialog(_container, new SettingsViewModel(_container));
        }

        private void OnApplicationEventMessage(object sender, ElapsedEventArgs e)
        {
            ApplicationEventMessage = "";
            _timer.Enabled = false;
        }
    }
}