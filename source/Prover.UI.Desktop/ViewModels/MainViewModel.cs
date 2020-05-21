using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Configuration;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;

namespace Prover.UI.Desktop.ViewModels
{
	public class MainViewModel : ReactiveObject, IDisposable
	{
		private readonly IConfiguration _config;

		public MainViewModel(IScreenManager screenManager, IToolbarManager toolbarManager, IConfiguration config
				)
		{
			_config = config;
			ScreenManager = screenManager;
			ToolbarManager = toolbarManager;
			//ToolbarItems = toolbarManager.ToolbarItems.OfType<IModuleToolbarItem>().OrderBy(t => t.SortOrder);

			//NavigateForward = ReactiveCommand.CreateFromTask<IRoutableViewModel, IRoutableViewModel>(ScreenManager.ChangeView);
			//NavigateBack = ReactiveCommand.CreateFromTask(ScreenManager.GoBack, ScreenManager.Router.NavigateBack.CanExecute);
			//NavigateHome = ReactiveCommand.CreateFromTask(async () =>
			//{
			//	await ScreenManager.GoHome();
			//});

		}

		[Reactive] public SnackbarMessageQueue MessageQueue { get; set; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));

		public string AppTitle { get; } = App.Title;

		public IScreenManager ScreenManager { get; }
		public IToolbarManager ToolbarManager { get; }
		//public IEnumerable<IModuleToolbarItem> ToolbarItems { get; }

		//public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> NavigateForward { get; }
		//public ReactiveCommand<Unit, Unit> NavigateBack { get; }
		//public ReactiveCommand<Unit, Unit> NavigateHome { get; }

		public void Dispose()
		{
			//NavigateForward?.Dispose();
			ScreenManager.Router.NavigationStack.Reverse().ForEach(vm => (vm as IDisposable)?.Dispose());
		}

		public void ShowHome(IRoutableViewModel homeViewModel)
		{
			ScreenManager.GoHome(homeViewModel);
		}
	}
}