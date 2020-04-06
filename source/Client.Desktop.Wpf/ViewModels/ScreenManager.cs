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
    internal class ScreenManager : ReactiveObject, IScreenManager
    {
        public IDialogServiceManager DialogManager { get; }
        [Reactive] public RoutingState Router { get; set; }
        public ScreenManager(IServiceProvider services, IDialogServiceManager dialogManager,
            Func<IScreenManager, IRoutableViewModel> homeViewModelFactory)
        {
            _services = services;

            Router = new RoutingState();

            DialogManager = dialogManager;
            _homeViewModel = homeViewModelFactory.Invoke(this);

            Router.CurrentViewModel.Subscribe(vm => _currentViewModel = vm);
        }

        public async Task<TViewModel> ChangeView<TViewModel>(TViewModel viewModel) where TViewModel : IRoutableViewModel
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            await Router.Navigate.Execute(viewModel);
            return viewModel;
        }

        public async Task<TViewModel> ChangeView<TViewModel>(params object[] parameters) where TViewModel : IRoutableViewModel
        {
            var model = 
                parameters.IsNullOrEmpty() 
                    ? _services.GetService<TViewModel>() 
                    : (TViewModel)ActivatorUtilities.CreateInstance(_services, typeof(TViewModel), parameters);

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
            await Router.NavigateAndReset.Execute(_homeViewModel);
        }

        private readonly IServiceProvider _services;
        private IRoutableViewModel _currentViewModel;
        private readonly IRoutableViewModel _homeViewModel;
    }
}