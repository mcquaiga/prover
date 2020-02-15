using System;
using System.Linq;
using System.Windows;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.EventLog;
using ReactiveUI;
using Splat;

namespace Client.Wpf
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private IHost _host;

        public App()
        {
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;

            ShutdownMode = ShutdownMode.OnLastWindowClose;
        }

        public IServiceProvider Container { get; private set; }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();
        }

        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }
        }

        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Console.WriteLine(args.LoadedAssembly);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            SetupApp();

            Locator.Current.GetService<IWindowFactory>()
                .Create()
                .Show();

            base.OnStartup(e);
        }

        public void SetupApp()
        {
            _host = Host
                .CreateDefaultBuilder()
                .ConfigureAppConfiguration(config => { AppBootstrapper.ConfigureAppConfiguration(config); })
                .ConfigureServices(async services =>
                {
                    services.UseMicrosoftDependencyResolver();

                    var resolver = Locator.CurrentMutable;
                    resolver.InitializeSplat();
                    resolver.InitializeReactiveUI();

                    await AppBootstrapper.ConfigureServices(services);
                })
                .ConfigureLogging(loggingBuilder =>
                {
                    
                    var eventLoggers = loggingBuilder.Services
                      .Where(l => l.ImplementationType == typeof(EventLogLoggerProvider))
                      .ToList();

                    foreach (var el in eventLoggers)
                        loggingBuilder.Services.Remove(el);
        

                })
                .UseEnvironment("Development")
                .Build();

            Container = _host.Services;
        }
    }
}