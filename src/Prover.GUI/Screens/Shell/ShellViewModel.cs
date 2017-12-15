using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Windows;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Prover.GUI.Events;
using Prover.GUI.Screens.Dialogs;
using Prover.GUI.Screens.MainMenu;
using Prover.GUI.Screens.Settings;
using Prover.GUI.Screens.Toolbar;
using ReactiveUI;

namespace Prover.GUI.Screens.Shell
{
    public class ShellViewModel : ReactiveConductor<ReactiveObject>.Collection.OneActive,
        IHandle<DialogDisplayEvent>, IDisposable
    {
        public IEnumerable<IToolbarItem> ToolbarItems { get; }
        public string Title => $"EVC Prover - v{GetVersionNumber()}";

        public ShellViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            MainMenuViewModel mainMenuViewModel, SettingsViewModel settingsViewModel,
            IEnumerable<IToolbarItem> toolBarItems)
        {
            eventAggregator.Subscribe(this);
            ToolbarItems = toolBarItems;
            screenManager.Conductor = this;

            GoHomeCommand = ReactiveCommand.Create(()
                => screenManager.ChangeScreen(mainMenuViewModel));

            OpenSettingsCommand = ReactiveCommand.Create(()
                => screenManager.ChangeScreen(settingsViewModel));

            RxApp.MainThreadScheduler = new DispatcherScheduler(Application.Current.Dispatcher);
        }

        public override void ActivateItem(ReactiveObject item)
        {
            if (ActiveItem != null)
            {
                var lastItem = ActiveItem;
                DeactivateItem(ActiveItem, true);
                if (ActiveItem != null)
                    return;

                (lastItem as IDisposable)?.Dispose();
            }

            base.ActivateItem(item);
        }

        public ReactiveCommand<Unit, Unit> OpenSettingsCommand { get; set; }
        public ReactiveCommand<Unit, Unit> GoHomeCommand { get; set; }

        private static string GetVersionNumber()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.ProductVersion;
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

        public void Dispose()
        {
        }
    }
}