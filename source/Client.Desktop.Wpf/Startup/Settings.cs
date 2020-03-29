using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prover.Application;
using Prover.Application.Services;
using Prover.Application.Settings;
using Prover.Shared.Interfaces;

namespace Client.Desktop.Wpf.Startup
{
    public class Settings : IHaveStartupTask, IHostedService
    {
        private readonly IServiceProvider _provider;

        public Settings(IServiceProvider provider)
        {
            _provider = provider;
        }

        public static void AddServices(IServiceCollection services, HostBuilderContext host)
        {
            services.AddSingleton(c => 
                ApplicationSettings.Initialize(AppDefaults.Settings.FilePath, c.GetService<IKeyValueStore>()));

            services.AddStartTask<Settings>();
            services.AddHostedService<Settings>();
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _provider.GetService<ISettingsService>().RefreshSettings();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _provider.GetService<ISettingsService>().SaveSettings();
        }
    }
}