using Client.Wpf.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Splat;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Client.Wpf.Startup;
using Microsoft.Extensions.Logging.EventLog;

namespace Client.Wpf
{
    internal static class Program
    {
        private static App _app;

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

            var task = StartAsync(args);
            HandleExceptions(task);

            _app.InitializeComponent();
            // Shows the Window specified by StartupUri
            _app.Run();
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
                _app.Shutdown();
            }
        }

        [STAThread]
        private static async Task StartAsync(string[] args)
        {
            using (var splashScreen = new StartScreen())
            {
                var booter = new AppBootstrapper();
                splashScreen.Show();

                var appHost = await Host.CreateDefaultBuilder()
                    .ConfigureHostConfiguration(builder =>
                    {
                    })
                    .ConfigureAppConfiguration(builder =>
                    {
                        booter.ConfigureAppConfiguration(builder);
                        booter.Config = builder.Build();
                    })
                    .ConfigureServices(services =>
                    {
                        booter.AddServices(services);
                    })
                    .ConfigureLogging(loggingBuilder =>
                    {
                        var eventLoggers = loggingBuilder.Services
                            .Where(l => l.ImplementationType == typeof(EventLogLoggerProvider))
                            .ToList();

                        foreach (var el in eventLoggers)
                            loggingBuilder.Services.Remove(el);
                    })
                    .UseEnvironment(args[0])
                    .StartAsync();

                var main = appHost.Services.GetService<IWindowFactory>().Create();

                main.Closed += (sender, args) =>
                {
                    _app.Shutdown();
                };

                main.Show();
                splashScreen.Owner = main;
                splashScreen.Close();
            }

            // This applies the XAML, e.g. StartupUri, Application.Resources
        }
    }
}