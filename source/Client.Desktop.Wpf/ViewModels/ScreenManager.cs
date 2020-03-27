using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Prover.Application.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels
{
    internal class ScreenManager : IScreenManager
    {
        private readonly IServiceProvider _services;
        private IRoutableViewModel _currentViewModel;

        public ScreenManager(IServiceProvider services, IDialogServiceManager dialogManager)
        {
            _services = services;

            Router = new RoutingState();

            DialogManager = dialogManager;

            Router.CurrentViewModel.Subscribe(vm => _currentViewModel = vm);
        }

        public RoutingState Router { get; }
        public IRoutableViewModel HomeViewModel { get; }
        //public extern IRoutableViewModel CurrentViewModel { [ObservableAsProperty] get; }
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
            var current = _currentViewModel;

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