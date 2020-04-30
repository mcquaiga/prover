using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prover.UI.Desktop.Common;
using Prover.UI.Desktop.Extensions;
using Splat;
using Squirrel;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Prover.UI.Desktop.Startup
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

            //var splatLogger = new RxLogging(_logger);
            //var splatLog = new WrappingFullLogger(splatLogger);
            //var logManager = Locator.Current.GetService<ILogManager>((string)null);

            //var sLog = logManager.GetLogger<UpdateManager>();

            //Locator.CurrentMutable.Register(() => splatLog, typeof(Splat.ILogger));
        }

        public static void AddServices(IServiceCollection services, HostBuilderContext host)
        {
            var path = host.Configuration.GetValue<string>(AppSettingsReleasesConfigKey);
            var cronTime = host.Configuration.GetValue<string>(AppSettingsUpdateScheduleKey);

            if (string.IsNullOrEmpty(path))
                return;

            services.AddHostedService(c =>
                ActivatorUtilities.CreateInstance<UpdaterService>(c,
                    path,
                    cronTime,
                    TimeZoneInfo.Local));

            services.AddSingleton<ILogManager, ProverLogManager>();
            //services.AddSingleton<ILogger<UpdateManager>>(c => )
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                await CheckForUpdate(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured");
                await StopAsync(cancellationToken);
                Dispose();
            }
        }

        private async Task CheckForUpdate(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            //using var mgr = await GetUpdateManager();

            //var updateInfo = await mgr.CheckForUpdate();
            //if (updateInfo.ReleasesToApply.Any())
            //{
            //    await mgr.UpdateApp();
            //    MessageBox.Show("Update applied.");
            //}
        }

        private async Task<IUpdateManager> GetUpdateManager()
        {
            await Task.CompletedTask;
            return null;
            //UpdateManager mgr;

            //if (_releasePath.Contains("github"))
            //{
            //    _logger.Log(LogLevel.Information, $"Checking for updates on GitHub");
            //    mgr = await UpdateManager.GitHubUpdateManager(_releasePath);
            //}
            //else
            //{
            //    _logger.Log(LogLevel.Information, $"Checking for updates at {_releasePath}");
            //    mgr = new UpdateManager(_releasePath);
            //}

            //return mgr;
        }
    }
}