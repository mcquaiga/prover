using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData.Binding;
using Microsoft.Extensions.DependencyInjection;
using Prover.Application.Interfaces;
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

		public async Task<TViewModel> ChangeView<TViewModel>(TViewModel viewModel) where TViewModel : IRoutableViewModel
		{
			var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
			await Router.Navigate.Execute(viewModel);
			return viewModel;
		}

		public async Task<TViewModel> ChangeView<TViewModel>(params object[] parameters) where TViewModel : IRoutableViewModel
		{
			var model = parameters.IsNullOrEmpty()
								? _services.GetService<TViewModel>()
								: ActivatorUtilities.CreateInstance<TViewModel>(_services, typeof(TViewModel), parameters);
			return await ChangeView(model);
		}

		public async Task GoBack()
		{
			var current = _currentViewModel;
			await Router.NavigateBack.Execute();
			//_toolbarRemover.Disposable = Disposable.Empty;
		}

		public async Task GoHome(IRoutableViewModel home = null)
		{
			_homeViewModel ??= home;
			if (_homeViewModel == null)
				throw new ArgumentNullException(nameof(home), @"No viewmodel has been set for home.");

			var stack = Router.NavigationStack.Reverse().SkipLast(1);

			await Router.NavigateAndReset.Execute(_homeViewModel);

			stack.ForEach(v => (v as IDisposable)?.Dispose());
		}
	}
}