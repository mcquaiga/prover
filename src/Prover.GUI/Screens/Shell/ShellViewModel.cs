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
using Prover.GUI.Common.Screens.MainMenu;
using Prover.GUI.Dialogs;
using Prover.GUI.Screens.Settings;

namespace Prover.GUI.Screens.Shell
{
    public class ShellViewModel : Conductor<object>.Collection.OneActive, IShell, IHandle<ScreenChangeEvent>
    {
        private readonly ScreenManager _screenManager;
        private readonly IEventAggregator _eventAggregator;
        private string _applicationEventMessage;
        private object _currentView;

        public ShellViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
        {
            _screenManager = screenManager;

            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
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

        //public void Handle(ConnectionStatusEvent message)
        //{
        //}

        //public void Handle(NotificationEvent message)
        //{
        //    _timer = new Timer(3000);
        //    ApplicationEventMessage = message.Message;
        //    _timer.Elapsed += OnApplicationEventMessage;
        //    _timer.Enabled = true;
        //}

        public void Handle(ScreenChangeEvent message)
        {
            if (_currentView != null)
            {
                DeactivateItem(_currentView, true);
            }
            
            ActivateItem(message.ViewModel);
            _currentView = message.ViewModel;
        }

        public async Task HomeButton()
        {
            await _screenManager.GoHome();
        }

        public void SettingsButton()
        {
            ShowSettingsWindow();
        }

        //public async Task LoginButton()
        //{
        //    await _container.Resolve<ILoginService>().Login();
        //}

        private void ShowSettingsWindow()
        {
            _screenManager.ShowDialog(new SettingsViewModel(_screenManager, _eventAggregator));
        }
    }
}