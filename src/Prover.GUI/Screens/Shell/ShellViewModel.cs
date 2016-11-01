using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Login;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;
using Prover.GUI.Common.Interfaces;
using Prover.GUI.Dialogs;
using Prover.GUI.Screens.Settings;

namespace Prover.GUI.Screens.Shell
{
    public class ShellViewModel : Conductor<object>.Collection.OneActive, IShell, IHandle<ScreenChangeEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ScreenManager _screenManager;
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

        public void Handle(ScreenChangeEvent message)
        {
            if (_currentView != null)
                DeactivateItem(_currentView, true);

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

        public async Task LoginButton()
        {
            await IoC.Get<ILoginService>().Login();
        }

        private void ShowSettingsWindow()
        {
            _screenManager.ShowDialog(new SettingsViewModel(_screenManager, _eventAggregator));
        }
    }
}