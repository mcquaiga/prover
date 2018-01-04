using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using NLog;
using Prover.Core.Settings;
using Prover.GUI.Events;
using Prover.GUI.Screens.Dialogs;
using ReactiveUI;
using Squirrel;
using LogManager = NLog.LogManager;

namespace Prover.GUI.Screens.Shell
{
    public class ShellViewModel : ReactiveConductor<ReactiveObject>.Collection.OneActive, IDisposable,
        IHandle<ScreenChangeEvent>, IHandle<NotificationEvent>, IHandle<DialogDisplayEvent>
    {
        private const string RepoUrl = "https://github.com/mcquaiga/EvcProver";
        private readonly ISettingsService _settingsService;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public IEnumerable<INavigationItem> NavigationItems { get; }
        public IEnumerable<IToolbarItem> ToolbarItems { get; }
        public string Title => $"EVC Prover - v{GetVersionNumber()}";

        public ShellViewModel(IEventAggregator eventAggregator,
            IEnumerable<INavigationItem> navigationItems, IEnumerable<IToolbarItem> toolBarItems, ISettingsService settingsService)
        {
            _settingsService = settingsService;
            eventAggregator.Subscribe(this);
            ToolbarItems = toolBarItems;

            NavigationItems = navigationItems.ToList().OrderByDescending(item => item.IsHome);
            GoHomeCommand = NavigationItems.First().NavigationCommand;

            WindowWidth = _settingsService.Local.WindowWidth;
            WindowHeight = _settingsService.Local.WindowHeight;
            WindowState = (WindowState) Enum.Parse(typeof(System.Windows.WindowState), _settingsService.Local.WindowState);

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

        public LocalSettings LocalSettings => _settingsService.Local;

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

        protected override void OnActivate()
        {
            Task.Run(async () => 
            {
                using (var mgr = await UpdateManager.GitHubUpdateManager(RepoUrl))
                {
                    var installed = mgr.CurrentlyInstalledVersion();
                    var latest = await mgr.UpdateApp();
                    if (latest != null && (installed == null || !latest.Version.Equals(installed)))
                    {
                        _log.Info("New update found.");
                    }
                }
            });

            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            _settingsService.Local.WindowHeight = WindowHeight;
            _settingsService.Local.WindowWidth = WindowWidth;
            _settingsService.Local.WindowState = WindowState.ToString();
            (ActiveItem as IDisposable)?.Dispose();

            base.OnDeactivate(close);
        }
       
        public void Dispose()
        {
            GoHomeCommand?.Dispose();
        }

        public void Handle(NotificationEvent message)
        {
            NotificationMessage = message.Message;
            ShowNotificationSnackbar = true;
        }

        public void Handle(ScreenChangeEvent message)
        {
            ActivateItem(message.ViewModel);
        }

        public void Handle(DialogDisplayEvent message)
        {
            DialogViewModel = message.ViewModel;
        }
    }
}