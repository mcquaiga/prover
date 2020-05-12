using DynamicData;
using Microsoft.Extensions.DependencyInjection;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Prover.Application.Extensions;

namespace Prover.UI.Desktop.ViewModels
{
	public class ToolbarManager : ReactiveObject, IToolbarManager
	{
		public ToolbarManager(IScreenManager screenManager, IEnumerable<IModuleToolbarItem> moduleToolbarItems)
		{

			_toolbarItems.Connect()
						 .LogDebug($"Toolbar Item added.")
						 //.ObserveOn(RxApp.MainThreadScheduler)
						 .Bind(out var toolbarItems)
						 .DisposeMany()
						 .Subscribe();

			ToolbarItems = toolbarItems;

			_toolbarItems.AddRange(moduleToolbarItems);


			NavigateForward = ReactiveCommand.CreateFromTask<IRoutableViewModel, IRoutableViewModel>(screenManager.ChangeView);
			NavigateBack = ReactiveCommand.CreateFromTask(screenManager.GoBack, screenManager.Router.NavigateBack.CanExecute);
			NavigateHome = ReactiveCommand.CreateFromTask(async () =>
			{
				await screenManager.GoHome();
			});
		}

		public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> NavigateForward { get; }
		public ReactiveCommand<Unit, Unit> NavigateBack { get; }
		public ReactiveCommand<Unit, Unit> NavigateHome { get; }

		public IDisposable AddToolbarItem(IToolbarItem item)
		{
			_toolbarItems.Add(item);

			return Disposable.Create(() =>
			{
				_toolbarItems.Remove(item);
			});
		}

		/// <inheritdoc />

		private readonly SourceList<IToolbarItem> _toolbarItems = new SourceList<IToolbarItem>();

		public ReadOnlyObservableCollection<IToolbarItem> ToolbarItems { get; } //=> _toolbarItems.Connect().AsObservableList();

		/*
		 * 	if (viewModel is IHaveToolbarItems barItems)
			{
				_toolbarRemover.Disposable = Disposable.Empty;

				var disposables = barItems.ToolbarActionItems.Select(AddToolbarItem).ToList();

				_toolbarRemover.Disposable = new CompositeDisposable(disposables.ToList());
				//.Prepend(viewModel as IDisposable));
			}
		 */

	}


	internal class ScreenManager : ReactiveObject, IScreenManager
	{
		private readonly SerialDisposable _toolbarRemover = new SerialDisposable();


		public IDialogServiceManager DialogManager { get; }
		[Reactive] public RoutingState Router { get; set; }
		public ScreenManager(IServiceProvider services, IDialogServiceManager dialogManager)
		{
			_services = services;
			DialogManager = dialogManager;

			Router = new RoutingState();

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

			return await ChangeView(model);
		}



		public async Task GoBack()
		{
			var current = _currentViewModel;

			await Router.NavigateBack.Execute();

			_toolbarRemover.Disposable = Disposable.Empty;
		}

		public async Task GoHome(IRoutableViewModel home = null)
		{
			_homeViewModel ??= home;

			if (_homeViewModel == null)
				throw new ArgumentNullException(nameof(home), @"No viewmodel has been set for home.");


			_toolbarRemover.Disposable = Disposable.Empty;

			//Router.NavigationStack.Reverse().ForEach(v => (v as IDisposable)?.Dispose());

			await Router.NavigateAndReset.Execute(_homeViewModel);
			//Router.NavigationStack.Reverse().Skip(1).ForEach(v =>
			//{
			//    (v as IDisposable)?.Dispose();
			//    Router.NavigateBack.Execute().Subscribe();
			//});
		}

		private readonly IServiceProvider _services;
		private IRoutableViewModel _currentViewModel;
		private IRoutableViewModel _homeViewModel;
	}
}