using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prover.Application;
using Prover.Shared.Interfaces;
using Prover.UI.Desktop.Common;
using Prover.UI.Desktop.Extensions;
using Prover.UI.Desktop.Startup;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using Splat;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using NullLogger = Microsoft.Extensions.Logging.Abstractions.NullLogger;

namespace Prover.UI.Desktop
{
	public class AppBootstrapper : IHostApplicationLifetime
	{
		public static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
		private ILogger _logger = NullLogger.Instance;

		public IHost AppHost { get; private set; }

		public CancellationToken ApplicationStarted { get; } = new CancellationToken();
		public CancellationToken ApplicationStopping { get; } = new CancellationToken();
		public CancellationToken ApplicationStopped { get; } = new CancellationToken();

		public async Task<AppBootstrapper> StartAsync(string[] args)
		{
			AppHost = ConfigureBuilder(this, args);

			await AppHost.StartAsync(CancellationTokenSource.Token);

			ProverLogging.Initialize(AppHost.Services);

			return this;
		}

		public void StopApplication()
		{
			_logger.LogDebug("Stopping Application.");
			AppHost.StopAsync(ApplicationStopped);
		}

		private static IHost ConfigureBuilder(AppBootstrapper booter, string[] args)
		{
			var host = Host.CreateDefaultBuilder()

					   .ConfigureHostConfiguration(config => { config.AddJsonFile("appsettings.json").AddJsonFile("appsettings.Development.json"); })

					   .ConfigureAppConfiguration((host, config) =>
					   {
						   host.DiscoverModules();
						   host.AddModuleConfigurations(config);
					   })

					   .ConfigureLogging((host, log) =>
					   {
						   log.ClearProviders();
						   log.Services.AddSplatLogging();

						   if (host.HostingEnvironment.IsProduction())
							   log.AddEventLog();
						   else
							   log.AddDebug();
					   })

					   .ConfigureServices((host, services) =>
					   {
						   services.AddSingleton<ISchedulerProvider, SchedulerProvider>();
						   services.AddSingleton<UnhandledExceptionHandler>();

						   host.AddModuleServices(services);

						   Settings.AddServices(services, host);
						   StorageStartup.AddServices(services, host);
						   UserInterface.AddServices(services, host);
						   DeviceServices.AddServices(services, host);

					   })
					   .Build();

			host.Services.UseMicrosoftDependencyResolver();

			return host;
		}

		//private void InitializeLogging()
		//{
		//	var logFactory = AppHost.Services.GetService<ILoggerFactory>();
		//	ProverLogging.LogFactory = logFactory;

		//	_logger = logFactory.CreateLogger<AppBootstrapper>();

		//	var config = AppHost.Services.GetService<IConfiguration>();
		//	var host = AppHost.Services.GetService<IHostEnvironment>();

		//	_logger.LogInformation($"Loggers Hosting App Name: {host.ApplicationName}");
		//	_logger.LogInformation($"Loggers Hosting Env: {host.EnvironmentName}");

		//	var level = config.GetValue<LogLevel>("Logging:LogLevel:Default");
		//	_logger.LogInformation($"Log Level = {level}");
		//}
	}
}