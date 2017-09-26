﻿using Caliburn.Micro;
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
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using Prover.GUI.Common.Screens.Dialogs;

namespace Prover.GUI.Screens.Shell
{
    public class ShellViewModel : ReactiveConductor<ReactiveObject>.Collection.OneActive, 
        IShell, 
        IHandle<ScreenChangeEvent>,
        IHandle<DialogDisplayEvent>
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

        public string Title => $"EVC Prover - v{Environment.Version}";

       
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
            DialogViewModel = message.ViewModel;          
        }       

        private IDialogViewModel _dialogViewModel;
        public IDialogViewModel DialogViewModel
        {
            get => _dialogViewModel;
            set => this.RaiseAndSetIfChanged(ref _dialogViewModel, value);
        }
    }
}