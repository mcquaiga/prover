using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;
using Prover.GUI.Common.Interfaces;
using Prover.GUI.Common.Screens.Toolbar;
using Prover.GUI.Dialogs;
using Prover.GUI.Screens.Settings;

namespace Prover.GUI.Screens.Shell
{
    public class ShellViewModel : Conductor<object>.Collection.OneActive, IShell, IHandle<ScreenChangeEvent>
    {
        public IEnumerable<IToolbarItem> ToolbarItems { get; set; }
        private readonly IEventAggregator _eventAggregator;
        private readonly ScreenManager _screenManager;
        private object _currentView;

        public ShellViewModel(ScreenManager screenManager, IEventAggregator eventAggregator, IEnumerable<IToolbarItem> toolbarItems)
        {
            ToolbarItems = toolbarItems;
            _screenManager = screenManager;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }

        public string Title => "EVC Prover";

        public ConnectionViewModel ConnectionDialogue { get; private set; }

        public void Handle(ScreenChangeEvent message)
        {
            if (_currentView != null)
            {
                DeactivateItem(_currentView, true);
                (_currentView as IDisposable)?.Dispose();
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

        private void ShowSettingsWindow()
        {
            _screenManager.ShowDialog(new SettingsViewModel(_screenManager, _eventAggregator));
        }
    }
}