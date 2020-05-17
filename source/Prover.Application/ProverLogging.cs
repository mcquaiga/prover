using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Prover.Application
{
	public class ProverLogging
	{
		private static ILoggerFactory _Factory = null;
		private static IConfiguration _config;
		private static IHostEnvironment _host;

		public static ILoggerFactory LogFactory
		{
			get
			{
				if (_Factory == null)
				{
					_Factory = CreateDefaultFactory();

				}
				return _Factory;
			}
			set { _Factory = value; }
		}

		public static void Initialize(IServiceProvider serviceProvider)
		{
			LogFactory = serviceProvider.GetService<ILoggerFactory>();

			_config = serviceProvider.GetService<IConfiguration>();
			_host = serviceProvider.GetService<IHostEnvironment>();

			LogLoggingInformation();
		}

		private static void LogLoggingInformation()
		{
			var logger = LogFactory.CreateLogger<ProverLogging>();

			logger.LogInformation($"***** Log Configuration ******");

			var level = _config.GetValue<LogLevel>("Logging:LogLevel:Default");
			logger.LogInformation($"Log Level: {level}");

			logger.LogInformation($"Hosting App Name: {_host.ApplicationName}");
			logger.LogInformation($"Hosting Env: {_host.EnvironmentName}");

		}

		private static ILoggerFactory CreateDefaultFactory()
		{
			return LoggerFactory.Create(builder =>
			{
				builder.AddDebug();
				builder.AddConsole();
			});
			//LoggerFactory.Create(builder => { 
			//builder
			//    .AddFilter("Microsoft", LogLevel.Warning)
			//    .AddFilter("System", LogLevel.Warning)
			//    .AddFilter("Prover.", LogLevel.Debug)
			//    .AddFilter("Devices", LogLevel.Information)
			//    .AddFilter("Client.", LogLevel.Trace)
			//    .AddDebug()
			//    .AddConsole(); });
		}

		public static ILogger CreateLogger(string categoryName) => LogFactory.CreateLogger(categoryName);
		public static ILogger CreateLogger(Type type) => LogFactory.CreateLogger(type);
		public static ILogger<T> CreateLogger<T>() => LogFactory.CreateLogger<T>();
	}
}