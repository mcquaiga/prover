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
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using MaterialDesignThemes.Wpf;
using Prover.GUI.Common.Screens.Dialogs;

namespace Prover.GUI.Screens.Shell
{
    public class ShellViewModel : ReactiveConductor<ReactiveObject>.Collection.OneActive, 
        IShell, 
        IHandle<ScreenChangeEvent>,
        IHandle<DialogDisplayEvent>
    {
        public IEnumerable<IToolbarItem> ToolbarItems { get; set; }
        readonly ScreenManager _screenManager;
        ReactiveObject _currentView;

        public ShellViewModel(ScreenManager screenManager, IEventAggregator eventAggregator, IEnumerable<IToolbarItem> toolbarItems)
        {
            ToolbarItems = toolbarItems;
            _screenManager = screenManager;
            eventAggregator.Subscribe(this);

            RxApp.MainThreadScheduler = new DispatcherScheduler(Application.Current.Dispatcher);
        }

        private static string GetVersionNumber()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.ProductVersion;
        }

        public string Title => $"EVC Prover - v{GetVersionNumber()}";
       
        public async Task HomeButton()
        {
            await _screenManager.GoHome();
        }

        public async Task SettingsButton()
        {
            await _screenManager.ChangeScreen<SettingsViewModel>(); //(new SettingsViewModel(_screenManager, _eventAggregator));
        }

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

        public void Handle(DialogDisplayEvent message)
        {
            Task.Run(() => DialogHost.Show(message.ViewModel));
        }       

        private IDialogViewModel _dialogViewModel;
        public IDialogViewModel DialogViewModel
        {
            get => _dialogViewModel;
            set => this.RaiseAndSetIfChanged(ref _dialogViewModel, value);
        }
    }
}