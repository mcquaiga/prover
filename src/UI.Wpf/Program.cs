using Client.Wpf.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Splat;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Windows.Threading;
using Client.Wpf.Startup;
using Client.Wpf.ViewModels;
using Devices.Communications.Interfaces;
using Devices.Communications.IO;
using Microsoft.Extensions.Logging.EventLog;

namespace Client.Wpf
{
    internal static class Program
    {
        private static App _app;
        private static AppBootstrapper _bootstrapper;
        private static IHost _host =>_bootstrapper.AppHost;
        private static MainWindow _mainWindow;

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            _app = new App();

            SynchronizationContext.SetSynchronizationContext(
                new DispatcherSynchronizationContext());

            _app.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var task = Initialize(args);
            HandleExceptions(task);
            using (IHost host = task.Result)
            {
                _app.InitializeComponent();
            // Shows the Window specified by StartupUri
                _app.Run();
            }
        }

        private static async void HandleExceptions(Task task)
        {
            try
            {
                await Task.Yield();
                await task;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _app.Shutdown();
            }
        }

        [STAThread]
        private static async Task<IHost> Initialize(string[] args)
        {
            using (var splashScreen = new StartScreen())
            {
                splashScreen.Show();
                
                _bootstrapper = await AppBootstrapper.StartAsync(args);


                splashScreen.Owner = LoadMainWindow();
                splashScreen.Close();
            }

            return _host;
        }

        private static MainWindow LoadMainWindow()
        {
            _mainWindow = _host.Services.GetService<MainWindow>();
            var model = _host.Services.GetService<MainViewModel>();

            _mainWindow.ViewModel = model;
            model.ShowMenu();

            _mainWindow.Closed += (sender, args) =>
            {
                (_mainWindow.ViewModel as IDisposable)?.Dispose();

                _bootstrapper.AppHost.Services.GetServices<IHostedService>().ToList()
                    .ForEach(h => h.StopAsync(new CancellationToken()));

                _app.Shutdown();
            };

            _mainWindow.InitializeComponent();

            _mainWindow.Show();

            return _mainWindow;
        }

        public static event EventHandler<EventArgs> ExitRequested;
        public static void viewModel_CloseRequested(object sender, EventArgs e)
        {
            _app.Shutdown();
        }

        private static void OnExitRequested(EventArgs e)
        {
            ExitRequested?.Invoke(_mainWindow, e);
        }
    }
}