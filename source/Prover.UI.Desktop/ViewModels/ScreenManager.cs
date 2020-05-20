using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData.Binding;
using Microsoft.Extensions.DependencyInjection;
using Prover.Application.Interfaces;
using Prover.Application.Verifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.UI.Desktop.ViewModels
{
	internal class ScreenManager : ReactiveObject, IScreenManager
	{
		private readonly IServiceProvider _services;
		//private readonly SerialDisposable _toolbarRemover = new SerialDisposable();
		private IRoutableViewModel _currentViewModel;
		private IRoutableViewModel _homeViewModel;

		public ScreenManager(IServiceProvider services, IDialogServiceManager dialogManager)
		{
			_services = services;

			DialogManager = dialogManager;

			Router = new RoutingState();
			Router.CurrentViewModel.Subscribe(vm => _currentViewModel = vm);

		}

		public IDialogServiceManager DialogManager { get; }

		public RoutingState Router { get; set; }

		public Task<TViewModel> ChangeView<TViewModel>(TViewModel viewModel) where TViewModel : IRoutableViewModel
		{
			return TryChangeView(viewModel);
		}

		public Task<TViewModel> ChangeView<TViewModel>(params object[] parameters) where TViewModel : IRoutableViewModel
		{
			if (_currentViewModel.GetType() == typeof(TViewModel))
				return default;

			var model = parameters.IsNullOrEmpty()
								? _services.GetService<TViewModel>()
								: ActivatorUtilities.CreateInstance<TViewModel>(_services, typeof(TViewModel), parameters);

			return ChangeView(model);
		}

		private Task<TViewModel> TryChangeView<TViewModel>(TViewModel viewModel) where TViewModel : IRoutableViewModel
		{
			var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
			if (_currentViewModel.GetType() == typeof(TViewModel))
				return default;

			_currentViewModel.

					Router.Navigate
				  .Execute(viewModel)
				  .Subscribe();

			return Task.FromResult(viewModel);
		}

		public async Task GoBack()
		{
			var current = _currentViewModel;
			await Router.NavigateBack.Execute();
			(current as IDisposable)?.Dispose();
		}

		public Task GoHome(IRoutableViewModel home = null)
		{
			_homeViewModel ??= home;

			if (_homeViewModel == null)
				throw new ArgumentNullException(nameof(home), @"No viewmodel has been set for home.");

			if (_currentViewModel == _homeViewModel)
				return Task.CompletedTask;

			//var stack = Router.NavigationStack.Reverse().SkipLast(1).ToList();

			Router.NavigateAndReset.Execute(_homeViewModel).Subscribe();

			//stack.ForEach(v => (v as IDisposable)?.Dispose());

			return Task.CompletedTask;
		}

		protected void NavigationChanging(IRoutableViewModel viewModel)
		{
			var cts = new CancellationTokenSource();
			if (viewModel is IViewModelNavigationEvents events)
			{

				events.ChangingNavigation.Register()
			}
		}
	}
}