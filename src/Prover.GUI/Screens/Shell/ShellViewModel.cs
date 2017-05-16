using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;
using Prover.GUI.Common.Interfaces;
using Prover.GUI.Common.Screens.Toolbar;
using Prover.GUI.Screens.Settings;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Windows;

namespace Prover.GUI.Screens.Shell
{
    public class ShellViewModel : ReactiveConductor<ReactiveObject>.Collection.OneActive, IShell, IHandle<ScreenChangeEvent>
    {
        public IEnumerable<IToolbarItem> ToolbarItems { get; set; }
        readonly IEventAggregator _eventAggregator;
        readonly ScreenManager _screenManager;
        ReactiveObject _currentView;

        public ShellViewModel(ScreenManager screenManager, IEventAggregator eventAggregator, IEnumerable<IToolbarItem> toolbarItems)
        {
            ToolbarItems = toolbarItems;
            _screenManager = screenManager;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            RxApp.MainThreadScheduler = new DispatcherScheduler(Application.Current.Dispatcher);
        }

        public string Title => "EVC Prover";

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