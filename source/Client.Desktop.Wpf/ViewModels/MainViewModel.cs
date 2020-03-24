using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Prover.Application.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels
{
    public partial class MainViewModel : ReactiveObject, IDisposable
    {
        //private readonly DialogServiceManager _dialogService;
        private readonly IServiceProvider _services;

        public MainViewModel(IServiceProvider services, IDialogServiceManager dialogManager, Func<IScreenManager, HomeViewModel> homeViewFactoryFunc)
        {
            _services = services;

            Router = new RoutingState();

            DialogManager = dialogManager;
           

            HomeViewModel = homeViewFactoryFunc.Invoke(this);

            GoNext = ReactiveCommand.CreateFromObservable<IRoutableViewModel, IRoutableViewModel>(Router.Navigate.Execute);
            NavigateBack = Router.NavigateBack;
            NavigateHome = ReactiveCommand.CreateFromTask(GoHome);
        }

        public string AppTitle { get; } = $"EVC Prover - v{GetVersionNumber()}";

        public IRoutableViewModel HomeViewModel { get; }
        public extern IRoutableViewModel CurrentViewModel { [ObservableAsProperty] get; }

        // The command that navigates a user to first view model.
        public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> GoNext { get; }
        public ReactiveCommand<Unit, Unit> NavigateBack { get; }
        public ReactiveCommand<Unit, Unit> NavigateHome { get; }

        public void Dispose()
        {
            GoNext?.Dispose();
            Router.NavigationStack.Reverse().ForEach(vm => (vm as IDisposable)?.Dispose());
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

    public partial class MainViewModel : IScreenManager
    {
        public RoutingState Router { get; }

        public IDialogServiceManager DialogManager { get; private set; }
        //public IViewLocator ViewLocator { get; }

        public async Task<IRoutableViewModel> ChangeView(IRoutableViewModel viewModel)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            await Router.Navigate.Execute(viewModel);
            return viewModel;
        }

        public async Task<IRoutableViewModel> ChangeView<TViewModel>() where TViewModel : IRoutableViewModel
        {
            var model = _services.GetService<TViewModel>();
            await Router.Navigate.Execute(model);
            return model;
        }

        public async Task GoBack()
        {
            var current = CurrentViewModel;

            await Router.NavigateBack.Execute();

            (current as IDisposable)?.Dispose();
        }

        public async Task GoHome()
        {
            Router.NavigationStack.Reverse().ForEach(v => (v as IDisposable)?.Dispose());
            await Router.NavigateAndReset.Execute(HomeViewModel);
        }
    }
}