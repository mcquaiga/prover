using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application;

namespace Client.Desktop.Wpf.Startup
{
    public class AppBootstrapper
    {
        public static CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        public IConfiguration Config { get; set; }

        public IHost AppHost { get; set; }
        private ILogger _logger = NullLogger.Instance;

        public void AddServices(IServiceCollection services, HostBuilderContext host)
        {
            Settings.AddServices(services, host);
            Storage.AddServices(services, host);
            UserInterface.AddServices(services, host);
            DeviceServices.AddServices(services, host);

            if (host.HostingEnvironment.EnvironmentName != Environments.Development)
            {
                //UpdaterService.AddServices(services, host);
            }
        }

        public void AddConfiguration(IConfigurationBuilder config, HostBuilderContext host)
        {
            config.SetBasePath(Directory.GetCurrentDirectory()); }


        public async Task<AppBootstrapper> StartAsync(string[] args)
        {
            AppHost = ConfigureBuilder(this, args);

            InitializeLogging();

            var startupTask = AppHost.Services.GetServices<IHaveStartupTask>().ToObservable().ForEachAsync(t => t.ExecuteAsync(CancellationTokenSource.Token));
            await startupTask;
            if (startupTask.IsFaulted)
            {
                foreach (var ex in startupTask.Exception.InnerExceptions)
                {
                    _logger.LogError(ex, "Errors occured on start up.");       
                }
            }

            await AppHost.StartAsync();

            return this;
        }

        private void InitializeLogging()
        {
            _logger = AppHost.Services.GetService<ILogger>();
            ProverLogging.LogFactory = AppHost.Services.GetService<ILoggerFactory>();
        }

        private static IHost ConfigureBuilder(AppBootstrapper booter, string[] args) =>
            Host.CreateDefaultBuilder()
               
                .ConfigureLogging((host, log) =>
                {

                    var logConfig = host.Configuration.GetSection("Logging");

                    log
                        .AddConfiguration(host.Configuration.GetSection("Logging"))
                        .AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("System", LogLevel.Warning)
                        .AddFilter("Prover.", LogLevel.Debug)
                        .AddFilter("Devices", LogLevel.Information)
                        .AddFilter("Client.", LogLevel.Trace)
                        .AddConsole()
                        .AddEventLog();

                    //log.Services.AddLogging();
                   

                })
                
                .ConfigureAppConfiguration((host, config) =>
                {
                    booter.AddConfiguration(config, host);
                })
                
                .ConfigureServices((host, services) =>
                {
                    services.AddSingleton<UnhandledExceptionHandler>();

                    host.ConfigureModules(services);

                    booter.AddServices(services, host);
                })
               
                .UseEnvironment(Environment.GetEnvironmentVariable("PROVER_ENVIRONMENT") ?? Environments.Production)
                
                .Build();
    }
}