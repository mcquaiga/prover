﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Prover.Application.Interfaces;
using Prover.Application.Verifications;
using ReactiveUI;

namespace Prover.UI.Desktop.ViewModels {
	//internal class ScreenRouter : RoutingState, IScreenManager
	//{
	//	/// <inheritdoc />
	//	public RoutingState Router => this;

	//	/// <inheritdoc />
	//	public IDialogServiceManager DialogManager { get; }

	//	/// <inheritdoc />
	//	public Task<TViewModel> ChangeView<TViewModel>(TViewModel viewModel) where TViewModel : IRoutableViewModel => throw new NotImplementedException();

	//	/// <inheritdoc />
	//	public Task<TViewModel> ChangeView<TViewModel>(params object[] parameters) where TViewModel : IRoutableViewModel => throw new NotImplementedException();

	//	/// <inheritdoc />
	//	public Task GoBack() => throw new NotImplementedException();

	//	/// <inheritdoc />
	//	public Task GoHome(IRoutableViewModel viewModel = null) => throw new NotImplementedException();
	//}


	internal class ScreenManager : RoutingState, IScreenManager, IDisposable {
		private readonly IServiceProvider _services;
		private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private IRoutableViewModel _homeViewModel;

		public ScreenManager(IServiceProvider services, IDialogServiceManager dialogManager) {
			_services = services;
			DialogManager = dialogManager;



			CanChangeNavigation = ReactiveCommand.CreateFromObservable<IRoutableViewModel, IRoutableViewModel>(viewModel => {
				var currentView = this.GetCurrentViewModel();

				if (currentView is IRoutableLifetimeHandler lifetimeHandler) {
					var linked = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, lifetimeHandler.OnChanging);

				}

				return Observable.Return(viewModel);
			});

			//Navigate = ReactiveCommand.CreateCombined(new[] { CanChangeNavigation, Navigate });
		}

		public IDialogServiceManager DialogManager { get; }

		public RoutingState Router => this;


		public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> CanChangeNavigation { get; set; }

		public Task<TViewModel> ChangeView<TViewModel>(TViewModel viewModel) where TViewModel : IRoutableViewModel => TryChangeView(viewModel);

		public Task<TViewModel> ChangeView<TViewModel>(params object[] parameters) where TViewModel : IRoutableViewModel {
			var model = parameters.IsNullOrEmpty() ? _services.GetService<TViewModel>() : ActivatorUtilities.CreateInstance<TViewModel>(_services, typeof(TViewModel), parameters);
			return TryChangeView(model);
		}

		/// <inheritdoc />
		public void Dispose() {
			(_homeViewModel as IDisposable)?.Dispose();
		}

		public Task GoBack() {
			var current = this.GetCurrentViewModel();
			NavigateBack.Execute().Subscribe();
			return Task.CompletedTask;
		}

		public Task GoHome(IRoutableViewModel home = null) {
			_homeViewModel = home ?? _homeViewModel;

			if (_homeViewModel == null)
				_homeViewModel = _services.GetService<HomeViewModel>();

			if (this.GetCurrentViewModel() == _homeViewModel)
				return Task.CompletedTask;

			NavigateAndReset.Execute(_homeViewModel).Subscribe();

			return Task.CompletedTask;
		}

		private bool CanChangeView<TViewModel>(TViewModel viewModel) where TViewModel : IRoutableViewModel {
			if (this.GetCurrentViewModel().GetType() == typeof(TViewModel))
				return false;
			return true;
		}

		private Task<TViewModel> TryChangeView<TViewModel>(TViewModel viewModel) where TViewModel : IRoutableViewModel {
			if (!CanChangeView(viewModel))
				return Task.FromResult<TViewModel>(default);

			if (viewModel is IRoutableLifetimeHandler handler) {
				//_cancellationTokenSource.
				//handler.OnChanging = _cancellationTokenSource.Token;
			}

			Navigate.Execute(viewModel).Subscribe();
			return Task.FromResult(viewModel);
		}
	}
}