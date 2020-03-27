using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reflection;
using Prover.Application.Interfaces;
using ReactiveUI;

namespace Client.Desktop.Wpf.ViewModels
{
    public partial class MainViewModel : ReactiveObject, IDisposable
    {
        public IScreenManager ScreenManager { get; }

        public MainViewModel(IScreenManager screenManager, HomeViewModel homeViewModel)
        {
            ScreenManager = screenManager;
            HomeViewModel = homeViewModel;

            GoNext = ReactiveCommand.CreateFromTask<IRoutableViewModel, IRoutableViewModel>(ScreenManager.ChangeView);
            NavigateBack = ReactiveCommand.CreateFromTask(ScreenManager.GoBack);
            NavigateHome = ReactiveCommand.CreateFromTask(ScreenManager.GoHome);
        }

        public string AppTitle { get; } = $"EVC Prover - v{GetVersionNumber()}";

        public IRoutableViewModel HomeViewModel { get; }

        // The command that navigates a user to first view model.
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
            GoNext.Execute(HomeViewModel);
        }

        private static string GetVersionNumber()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.FileVersion;
        }
    }


    //public partial class MainViewModel : IScreenManager
    //{
    //    public RoutingState Router { get; }

    //    public IDialogServiceManager DialogManager { get; private set; }
    //    //public IViewLocator ViewLocator { get; }

    //    public async Task<IRoutableViewModel> ChangeView(IRoutableViewModel viewModel)
    //    {
    //        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
    //        await Router.Navigate.Execute(viewModel);
    //        return viewModel;
    //    }

    //    public async Task<IRoutableViewModel> ChangeView<TViewModel>() where TViewModel : IRoutableViewModel
    //    {
    //        var model = _services.GetService<TViewModel>();
    //        await Router.Navigate.Execute(model);
    //        return model;
    //    }

    //    public async Task GoBack()
    //    {
    //        var current = CurrentViewModel;

    //        await Router.NavigateBack.Execute();

    //        (current as IDisposable)?.Dispose();
    //    }

    //    public async Task GoHome()
    //    {
    //        Router.NavigationStack.Reverse().ForEach(v => (v as IDisposable)?.Dispose());
    //        await Router.NavigateAndReset.Execute(HomeViewModel);
    //    }
    //}
}