using System;
using Microsoft.Extensions.DependencyInjection;
using Prover.UI.Desktop.ViewModels;

namespace Prover.UI.Desktop.Views {
	public class WindowFactory : IWindowFactory {
		private readonly IServiceProvider _resolver;

		public WindowFactory(IServiceProvider resolver) {
			_resolver = resolver;
		}

		public MainWindow Create(bool showMenu = true) {
			var window = new MainWindow();
			var model = _resolver.GetService<MainViewModel>();

			window.ViewModel = model;

			if (showMenu) {
				var home = _resolver.GetService<HomeViewModel>();
				//model.ShowHome(home);
			}


			window.Closing += (sender, e) => {
				(((MainWindow)sender).ViewModel as IDisposable)?.Dispose();
			};

			return window;
		}
	}

	public interface IWindowFactory {
		MainWindow Create(bool showMenu = true);
	}
}
