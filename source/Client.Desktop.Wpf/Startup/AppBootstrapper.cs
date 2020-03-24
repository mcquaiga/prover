using System.Diagnostics;
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
    public class AppBootstrapper : IHostApplicationLifetime
    {
        public static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
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

            UpdaterService.AddServices(services, host);
        }


        public async Task<AppBootstrapper> StartAsync(string[] args)
        {
            AppHost = ConfigureBuilder(this, args);
            await AppHost.StartAsync();

            await ExecuteStartUpTasks();

            InitializeLogging();

            return this;
        }

        private async Task ExecuteStartUpTasks()
        {
            var startupTask = AppHost.Services.GetServices<IHaveStartupTask>().ToObservable()
                .ForEachAsync(async t => await t.ExecuteAsync(CancellationTokenSource.Token));
            await startupTask;

            if (startupTask.IsFaulted)
                foreach (var ex in startupTask.Exception.InnerExceptions)
                    _logger.LogError(ex, "Errors occured on start up.");
        }

        private static IHost ConfigureBuilder(AppBootstrapper booter, string[] args) =>
            Host.CreateDefaultBuilder()
                .ConfigureLogging((host, log) =>
                {
                    log.AddEventLog();
#if DEBUG
                    log.Services.AddSplatLogging();
                    log.AddDebug();
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
            var logFactory = AppHost.Services.GetService<ILoggerFactory>();
            ProverLogging.LogFactory = logFactory;

            _logger = logFactory.CreateLogger<AppBootstrapper>();
            
            var config = AppHost.Services.GetService<IConfiguration>();
            //var logConfig = config.GetSection("Logging").GetChildren();

            var host = AppHost.Services.GetService<IHostEnvironment>();
            _logger.LogInformation($"Loggers Hosting App Name: {host.ApplicationName}");
            _logger.LogInformation($"Loggers Hosting Env: {host.EnvironmentName}");

            var level = config.GetValue<LogLevel>("Logging:LogLevel:Default");
            _logger.LogInformation($"Log Level = {level}");

            //_logger.LogInformation($"Logger Settings");
            //_logger.LogInformation($"Log Level = {config.GetValue("Logging:LogLevel:Default", typeof(LogLevel))}");
            //foreach (var section in logConfig)
            //{
            //    _logger.LogInformation($"{section} = {section}");
            //}
            
        }

        public void StopApplication()
        {
            Debug.WriteLine("Stopping Application.");
        }

        public CancellationToken ApplicationStarted { get; }
        public CancellationToken ApplicationStopping { get; }
        public CancellationToken ApplicationStopped { get; }
    }

}