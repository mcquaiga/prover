using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Prover.Core.Settings;
using Prover.GUI.Events;
using Prover.GUI.Screens.Dialogs;
using Prover.GUI.Screens.MainMenu;
using Prover.GUI.Screens.Settings;
using Prover.GUI.Screens.Toolbar;
using ReactiveUI;

namespace Prover.GUI.Screens.Shell
{
    public class ShellViewModel : ReactiveConductor<ReactiveObject>.Collection.OneActive,
        IHandle<ScreenChangeEvent>, IDisposable
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

            _windowWidth = SettingsManager.LocalSettingsInstance.WindowWidth;
            _windowHeight = SettingsManager.LocalSettingsInstance.WindowHeight;

            this.WhenAnyValue(x => x.WindowHeight)
                .Subscribe(h => SettingsManager.LocalSettingsInstance.WindowHeight = h);

            this.WhenAnyValue(x => x.WindowWidth)
                .Subscribe(w => SettingsManager.LocalSettingsInstance.WindowWidth = w);

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

        public void Dispose()
        {
            (ActiveItem as IDisposable)?.Dispose();
        }

        public void Handle(ScreenChangeEvent message)
        {
            ActivateItem(message.ViewModel);
        }
    }
}