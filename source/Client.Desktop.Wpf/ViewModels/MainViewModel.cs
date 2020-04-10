using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using Microsoft.Extensions.Configuration;
using Prover.Application.Interfaces;
using ReactiveUI;

namespace Client.Desktop.Wpf.ViewModels
{
    public class PopupMenuToolbar : ReactiveObject, IToolbarItem
    {

    }

    public class MainViewModel : ReactiveObject, IDisposable
    {
        private readonly IConfiguration _config;

        public MainViewModel(IScreenManager screenManager, IEnumerable<IToolbarItem> toolbarItems, IConfiguration config)
        {
            _config = config;
            ScreenManager = screenManager;
            ToolbarItems = toolbarItems;

            GoNext = ReactiveCommand.CreateFromTask<IRoutableViewModel, IRoutableViewModel>(ScreenManager.ChangeView);
            NavigateBack = ReactiveCommand.CreateFromTask(ScreenManager.GoBack, ScreenManager.Router.NavigateBack.CanExecute);
            NavigateHome = ReactiveCommand.CreateFromTask(async () => await ScreenManager.GoHome());
         
        }

        public string AppTitle { get; } = App.Title;

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

        public void ShowHome(IRoutableViewModel homeViewModel)
        {
            ScreenManager.GoHome(homeViewModel);
        }
    }
}