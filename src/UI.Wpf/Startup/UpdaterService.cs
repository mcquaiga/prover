using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using Client.Wpf.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Squirrel;

namespace Client.Wpf.Startup
{
    public class UpdaterService : CronJobService
    {
        private const string AppSettings_ReleasesConfigKey = "Releases:Path";
        private const string AppSettings_UpdateScheduleKey = "Releases:UpdateSchedule";
        private readonly ILogger<UpdaterService> _logger;
        private readonly string _releasePath;

        public UpdaterService(ILogger<UpdaterService> logger, string releasePath, string cronExpression, TimeZoneInfo timeZoneInfo) 
            : base(cronExpression, timeZoneInfo)
        {
            _logger = logger;
            _releasePath = releasePath;
        }

        public static void AddServices(IServiceCollection services, IConfiguration config)
        {
            var path = config.GetValue<string>(AppSettings_ReleasesConfigKey);
            var cronTime = config.GetValue<string>(AppSettings_UpdateScheduleKey);

            services.AddHostedService(c => 
                ActivatorUtilities.CreateInstance<UpdaterService>(c, 
                    path,
                    cronTime,
                    TimeZoneInfo.Local));
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.Log(LogLevel.Debug, $"Checking for updates...");

            try
            {
                using (var mgr = new UpdateManager(_releasePath))
                {
                    _logger.Log(LogLevel.Debug, $"Update found...");
                    await mgr.UpdateApp();
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occured");
                await StopAsync(cancellationToken);
                Dispose();
            }

        }
    }
}