using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Prover.Core.Settings;
using Prover.GUI.Events;
using Prover.GUI.Screens.Dialogs;
using ReactiveUI;

namespace Prover.GUI.Screens.Shell
{
    public class ShellViewModel : ReactiveConductor<ReactiveObject>.Collection.OneActive,
        IHandle<ScreenChangeEvent>, 
        IHandle<NotificationEvent>,
        IHandle<DialogDisplayEvent>, IDisposable
    {
        public IEnumerable<INavigationItem> NavigationItems { get; }
        public IEnumerable<IToolbarItem> ToolbarItems { get; }
        public string Title => $"EVC Prover - v{GetVersionNumber()}";

        public ShellViewModel(IEventAggregator eventAggregator,
            IEnumerable<INavigationItem> navigationItems, IEnumerable<IToolbarItem> toolBarItems)
        {
            eventAggregator.Subscribe(this);
            ToolbarItems = toolBarItems;

            NavigationItems = navigationItems.ToList().OrderByDescending(item => item.IsHome);
            GoHomeCommand = NavigationItems.First().NavigationCommand;

            WindowWidth = SettingsManager.LocalSettingsInstance.WindowWidth;
            WindowHeight = SettingsManager.LocalSettingsInstance.WindowHeight;
            WindowState = (WindowState) Enum.Parse(typeof(System.Windows.WindowState), SettingsManager.LocalSettingsInstance.WindowState);

            this.WhenAnyValue(x => x.ShowNotificationSnackbar);                

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

        public ReactiveCommand<Unit, Unit> GoHomeCommand { get; set; }

        private static string GetVersionNumber()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.ProductVersion;
        }

        #region Properties        

        public LocalSettings LocalSettings => SettingsManager.LocalSettingsInstance;

        private IDialogViewModel _dialogViewModel;
        public IDialogViewModel DialogViewModel
        {
            get => _dialogViewModel;
            set => this.RaiseAndSetIfChanged(ref _dialogViewModel, value);
        }

        private double _windowHeight;
        public double WindowHeight
        {
            get => _windowHeight;
            set => this.RaiseAndSetIfChanged(ref _windowHeight, value);
        }

        private double _windowWidth;
        public double WindowWidth
        {
            get => _windowWidth;
            set => this.RaiseAndSetIfChanged(ref _windowWidth, value);
        }

        private WindowState _windowState;
        public WindowState WindowState
        {
            get => _windowState;
            set => this.RaiseAndSetIfChanged(ref _windowState, value);
        }

        private bool _showNotificationSnackbar;
        public bool ShowNotificationSnackbar
        {
            get => _showNotificationSnackbar;
            set => this.RaiseAndSetIfChanged(ref _showNotificationSnackbar, value);
        }

        private string _notificationMessage;
        public string NotificationMessage
        {
            get => _notificationMessage;
            set => this.RaiseAndSetIfChanged(ref _notificationMessage, value);
        }
        #endregion

        public void Handle(ScreenChangeEvent message)
        {
            ActivateItem(message.ViewModel);
        }

        protected override void OnDeactivate(bool close)
        {
            SettingsManager.LocalSettingsInstance.WindowHeight = WindowHeight;
            SettingsManager.LocalSettingsInstance.WindowWidth = WindowWidth;
            SettingsManager.LocalSettingsInstance.WindowState = WindowState.ToString();
            (ActiveItem as IDisposable)?.Dispose();

            base.OnDeactivate(close);
        }

        public void Handle(NotificationEvent message)
        {
            NotificationMessage = message.Message;
            ShowNotificationSnackbar = true;
        }

        public void Dispose()
        {
            GoHomeCommand?.Dispose();
        }

        public void Handle(DialogDisplayEvent message)
        {
            DialogViewModel = message.ViewModel;
        }
    }
}