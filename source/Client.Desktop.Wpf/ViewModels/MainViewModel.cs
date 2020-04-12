using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Configuration;
using Prover.Application.Dashboard;
using Prover.Application.Interfaces;
using ReactiveUI;

namespace Client.Desktop.Wpf.ViewModels
{
    public class MainViewModel : ReactiveObject, IDisposable
    {
        private readonly IConfiguration _config;

        public MainViewModel(IScreenManager screenManager, IEnumerable<IToolbarItem> toolbarItems, 
                IConfiguration config
                )
        {
            _config = config;
            ScreenManager = screenManager;
            ToolbarItems = toolbarItems;

            NavigateForward = ReactiveCommand.CreateFromTask<IRoutableViewModel, IRoutableViewModel>(ScreenManager.ChangeView);
            NavigateBack = ReactiveCommand.CreateFromTask(ScreenManager.GoBack, ScreenManager.Router.NavigateBack.CanExecute);
            NavigateHome = ReactiveCommand.CreateFromTask(async () => await ScreenManager.GoHome());
         
        }

        public SnackbarMessageQueue MessageQueue { get; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));

        public string AppTitle { get; } = App.Title;

        public IScreenManager ScreenManager { get; }
        public IEnumerable<IToolbarItem> ToolbarItems { get; }

        public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> NavigateForward { get; }
        public ReactiveCommand<Unit, Unit> NavigateBack { get; }
        public ReactiveCommand<Unit, Unit> NavigateHome { get; }

        public void Dispose()
        {
            NavigateForward?.Dispose();
            ScreenManager.Router.NavigationStack.Reverse().ForEach(vm => (vm as IDisposable)?.Dispose());
        }

        public void ShowHome(IRoutableViewModel homeViewModel)
        {
            ScreenManager.GoHome(homeViewModel);
        }
    }
}