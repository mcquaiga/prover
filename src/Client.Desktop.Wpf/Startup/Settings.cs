using System;
using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.Services;
using Application.Settings;
using Client.Wpf.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Interfaces;

namespace Client.Wpf.Startup
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

        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _provider.GetService<ISettingsService>().SaveSettings();
        }
    }
}