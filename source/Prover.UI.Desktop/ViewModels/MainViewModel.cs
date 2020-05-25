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

namespace Prover.UI.Desktop.ViewModels {
	public class MainViewModel : ReactiveObject, IDisposable {
		private readonly IConfiguration _config;

		public MainViewModel(IScreenManager screenManager, IToolbarManager toolbarManager, IConfiguration config) {
			_config = config;
			ScreenManager = screenManager;
			ToolbarManager = toolbarManager;

			NavigateHome = ReactiveCommand.CreateFromTask(() => ScreenManager.GoHome());
		}

		[Reactive] public SnackbarMessageQueue MessageQueue { get; set; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
		public ReactiveCommand<Unit, Unit> NavigateHome { get; set; }
		public string AppTitle { get; } = App.Title;

		public IScreenManager ScreenManager { get; }
		public IToolbarManager ToolbarManager { get; }

		public void Dispose() {
			(ScreenManager as IDisposable)?.Dispose();
			(ToolbarManager as IDisposable)?.Dispose();
			//MessageQueue?.Dispose();
		}
	}
}