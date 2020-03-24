using Client.Desktop.Wpf.Startup;
using Client.Desktop.Wpf.ViewModels;
using Client.Desktop.Wpf.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Client.Desktop.Wpf
{
    internal static class Program
    {
        private static readonly App App = new App {ShutdownMode = ShutdownMode.OnExplicitShutdown};
        private static MainWindow _mainWindow;
        private static IHost _host;

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());
            
            using (var splashScreen = new StartScreen())
            {
                splashScreen.Show();

                var task = Initialize(args);
                HandleExceptions(task);
                _host = task.Result;

                splashScreen.Owner = LoadMainWindow();
                splashScreen.Close();
            }

            App.InitializeComponent();
            _host.Services.GetService<UnhandledExceptionHandler>();
            App.Run();
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
                App.Shutdown();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                App.Shutdown();
            }
        }

        //[STAThread]
        private static async Task<IHost> Initialize(string[] args)
        {
            var bootstrapper = await new AppBootstrapper().StartAsync(args);
           
            return bootstrapper.AppHost;
        }

        private static MainWindow LoadMainWindow()
        {
            var model = _host.Services.GetService<MainViewModel>();
            _mainWindow = _host.Services.GetService<MainWindow>();
            _mainWindow.ViewModel = model;

            model.ShowHome();

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
            App.Shutdown();
        }

        private static void ShutdownHostedServices()
        {
            using (_host)
            {
                var services = _host.Services.GetServices<IHostedService>().ToList();

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