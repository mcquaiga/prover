using Microsoft.Extensions.Logging;

namespace Prover.DevTools
{
	public static class DevLogger
	{
		public static ILoggerFactory Factory = LoggerFactory.Create(builder =>
		{
			builder.AddConsole();
			builder.AddDebug();
			builder.SetMinimumLevel(LogLevel.Debug);

		});

		public static ILogger Logger { get; } = Factory.CreateLogger("DevTools");
	}
}