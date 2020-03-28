using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using Prover.Application.Interfaces;
using ReactiveUI;

namespace Client.Desktop.Wpf.ViewModels
{
    public class MainViewModel : ReactiveObject, IDisposable
    {
        public MainViewModel(IScreenManager screenManager, IEnumerable<IToolbarItem> toolbarItems)
        {
            ScreenManager = screenManager;
            ToolbarItems = toolbarItems;

            GoNext = ReactiveCommand.CreateFromTask<IRoutableViewModel, IRoutableViewModel>(ScreenManager.ChangeView);
            NavigateBack = ReactiveCommand.CreateFromTask(ScreenManager.GoBack);
            NavigateHome = ReactiveCommand.CreateFromTask(ScreenManager.GoHome);
        }

        public string AppTitle { get; } = $"EVC Prover - v{App.VersionNumber}";

        public IScreenManager ScreenManager { get; }
        public IEnumerable<IToolbarItem> ToolbarItems { get; }

        public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> GoNext { get; }
        public ReactiveCommand<Unit, Unit> NavigateBack { get; }
        public ReactiveCommand<Unit, Unit> NavigateHome { get; }

        public void Dispose()
        {
            GoNext?.Dispose();
            ScreenManager.Router.NavigationStack.Reverse().ForEach(vm => (vm as IDisposable)?.Dispose());
        }

        public void ShowHome()
        {
            ScreenManager.GoHome();
        }
    }
}