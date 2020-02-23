using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Client.Desktop.Wpf.ViewModels;
using Client.Wpf.Startup;
using Client.Wpf.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Client.Wpf
{
    internal static class Program
    {
        private static App _app;
        private static AppBootstrapper _bootstrapper;
        private static MainWindow _mainWindow;
        private static IHost _host => _bootstrapper.AppHost;

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            _app = new App();

            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());

            _app.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var task = Initialize(args);
            HandleExceptions(task);

            try
            {
                _app.InitializeComponent();
                _app.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Unhandled error occured. {Environment.NewLine} Exception: {ex.Message} {Environment.NewLine} {ex.InnerException?.Message}.");
                Console.WriteLine(ex.Message);
                ShutdownApp();
            }
        }

        public static event EventHandler<EventArgs> ExitRequested;

        private static async void HandleExceptions(Task task)
        {
            try
            {
                await Task.Yield();
                await task;
            }
            catch (AggregateException aggEx)
            {
                foreach (var ex in aggEx.InnerExceptions) Console.WriteLine(ex.Message);
                _app.Shutdown();
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

                _bootstrapper = await new AppBootstrapper().StartAsync(args);

                splashScreen.Owner = LoadMainWindow();
                splashScreen.Close();
            }

            return _host;
        }

        private static MainWindow LoadMainWindow()
        {
            var model = _host.Services.GetService<MainViewModel>();
            _mainWindow = _host.Services.GetService<MainWindow>();
            _mainWindow.ViewModel = model;

            model.ShowMenu();

            _mainWindow.Closed += (sender, args) => { ShutdownApp(); };

            _mainWindow.InitializeComponent();
            _mainWindow.Show();

            return _mainWindow;
        }

        private static void OnExitRequested(EventArgs e)
        {
            ExitRequested?.Invoke(_mainWindow, e);
        }

        private static void ShutdownApp()
        {
            (_mainWindow?.ViewModel as IDisposable)?.Dispose();
            ShutdownHostedServices();
            _app.Shutdown();
        }

        private static void ShutdownHostedServices()
        {
            using (_host)
            {
                var services = _bootstrapper.AppHost.Services.GetServices<IHostedService>().ToList();

                var t = Task.WhenAll(services.Select(s =>
                    Task.Run(() => s.StopAsync(new CancellationToken()))));

                t.Wait();
            }
        }

        private static void viewModel_CloseRequested(object sender, EventArgs e)
        {
            ShutdownApp();
        }
    }
}