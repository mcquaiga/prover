using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Wpf.Startup
{
    public class UpdaterService : CronJobService
    {
        private const string AppSettingsReleasesConfigKey = "Releases:Path";
        private const string AppSettingsUpdateScheduleKey = "Releases:UpdateSchedule";
        private readonly ILogger<UpdaterService> _logger;
        private readonly string _releasePath;

        public UpdaterService(ILogger<UpdaterService> logger, string releasePath, string cronExpression, TimeZoneInfo timeZoneInfo) 
            : base(cronExpression, timeZoneInfo)
        {
            _logger = logger;
            _releasePath = releasePath;
        }

        public static void AddServices(IServiceCollection services, HostBuilderContext host)
        {
            var path = host.Configuration.GetValue<string>(AppSettingsReleasesConfigKey);
            var cronTime = host.Configuration.GetValue<string>(AppSettingsUpdateScheduleKey);

            services.AddHostedService(c => 
                ActivatorUtilities.CreateInstance<UpdaterService>(c, 
                    path,
                    cronTime,
                    TimeZoneInfo.Local));
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                await CheckForUpdate(cancellationToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occured");
                await StopAsync(cancellationToken);
                Dispose();
            }

        }

        private async Task CheckForUpdate(CancellationToken cancellationToken)
        {
            using var mgr = await GetUpdateManager();
            await mgr.UpdateApp();
        }

        private async Task<IUpdateManager> GetUpdateManager()
        {
            IUpdateManager mgr;

            if (_releasePath.Contains("github"))
            {
                _logger.Log(LogLevel.Information, $"Checking for updates on GitHub");
                mgr = await UpdateManager.GitHubUpdateManager(_releasePath);
            }
            else
            {
                _logger.Log(LogLevel.Information, $"Checking for updates at {_releasePath}");
                mgr = new UpdateManager(_releasePath);
            }

            return mgr;
        }
    }
}