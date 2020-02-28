using System;
using System.Diagnostics;
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
using Prover.Shared.Interfaces;

namespace Client.Desktop.Wpf.Startup
{
    public class AppBootstrapper
    {
        public static CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        private ILogger _logger = NullLogger.Instance;
        public IConfiguration Config { get; set; }

        public IHost AppHost { get; set; }

        public void AddConfiguration(IConfigurationBuilder config, HostBuilderContext host)
        {
            
        }

        public void AddServices(IServiceCollection services, HostBuilderContext host)
        {
            services.AddSingleton<ISchedulerProvider, SchedulerProvider>();
            services.AddSingleton<UnhandledExceptionHandler>();

            Settings.AddServices(services, host);
            Storage.AddServices(services, host);
            UserInterface.AddServices(services, host);
            DeviceServices.AddServices(services, host);

            if (host.HostingEnvironment.EnvironmentName != Environments.Development)
            {
                //UpdaterService.AddServices(services, host);
            }
        }


        public async Task<AppBootstrapper> StartAsync(string[] args)
        {
            AppHost = ConfigureBuilder(this, args);

            InitializeLogging();

            var startupTask = AppHost.Services.GetServices<IHaveStartupTask>().ToObservable()
                .ForEachAsync(t => t.ExecuteAsync(CancellationTokenSource.Token));
            await startupTask;
            if (startupTask.IsFaulted)
                foreach (var ex in startupTask.Exception.InnerExceptions)
                    _logger.LogError(ex, "Errors occured on start up.");

            await AppHost.StartAsync();

            return this;
        }

        private static IHost ConfigureBuilder(AppBootstrapper booter, string[] args) =>
            Host.CreateDefaultBuilder()
                //.UseEnvironment(Environment.GetEnvironmentVariable("PROVER_ENVIRONMENT") ?? Environments.Production)
                //.ConfigureAppConfiguration((host, config) =>
                //{
                //    //config.AddJsonFile();
                //    //config.SetBasePath(Directory.GetCurrentDirectory());

                //})
                .ConfigureLogging((host, log) =>
                {
                    log.ClearProviders();
                    log.AddConfiguration((IConfiguration) host.Configuration.GetSection("Logging"));
                    log.Services.AddLogging();
#if DEBUG
                    //log.AddDebug();
                    log.Services.AddSplatLogging();
                    //log.AddFilter("ReactiveUI", LogLevel.Debug)
                    //    .AddDebug();
#endif
                })
                .ConfigureServices((host, services) =>
                {
                    booter.AddServices(services, host);
                    host.ConfigureModules(services);
                    
                })
                .Build();

        private void InitializeLogging()
        {
            _logger = AppHost.Services.GetService<ILogger>();
            var logFactory = AppHost.Services.GetService<ILoggerFactory>();
            ProverLogging.LogFactory = logFactory;
            
            AppHost.Services.GetService<ILoggerFactory>();
            var config = AppHost.Services.GetService<IConfiguration>();

            var logConfig = config.GetSection("Logging").GetChildren();
        }
    }

}