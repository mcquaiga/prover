﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Prover.Wpf {
	internal static class Program {
		private static App _app;

		/// <summary>
		///     The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main(string[] args) {
			SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());

			var startup = App.Starting(async () => {
				await AppBootstrapper.StartAsync(args);
				return AppBootstrapper.Instance;
			});
			HandleExceptions(startup);
			_app = startup.Result;

			//AppBootstrapper.Instance.LifetimeHost.ApplicationStopped.Register(async () => {
			//	//await AppBootstrapper.Instance.StopAsync();
			//	//App.Shutdown();
			//}, true);

			_app.AppMainWindow
			   .Events().Closed
			   .Select(_ => Unit.Default)
			   .InvokeCommand(ReactiveCommand.CreateFromTask(Shutdown));
			AppBootstrapper.Instance.AppHost.Services.GetService<UnhandledExceptionHandler>();
			_app.Run();
		}
		private static async void HandleExceptions(Task task) {
			try {
				await Task.Yield();
				await task;
			}
			catch (AggregateException aggEx) {
				foreach (var ex in aggEx.InnerExceptions)
					Debug.WriteLine(ex.Message);
				_app.Shutdown();
			}

			catch (Exception ex) {
				Debug.WriteLine(ex.Message);
				_app.Shutdown();
			}
		}

		public static Task Shutdown() {

			using (var host = AppBootstrapper.Instance) {
				(_app.AppMainWindow as IDisposable)?.Dispose();
				host.StopAsync();
				_app.Shutdown();
			}
			return Task.CompletedTask;
		}
	}
}