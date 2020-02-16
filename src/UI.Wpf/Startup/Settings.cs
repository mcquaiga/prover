using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Services;
using Application.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Interfaces;

namespace Client.Wpf.Startup
{
    public class Settings : IHostedService
    {
        private readonly IServiceProvider _provider;

        public Settings(IServiceProvider provider)
        {
            _provider = provider;
        }

        public static void AddServices(IServiceCollection services, IConfiguration config)
        {
            services.AddHostedService<Settings>();

            services.AddSingleton(c => 
                ApplicationSettings.Initialize(c.GetService<IConfiguration>(), c.GetService<IKeyValueStore>()));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _provider.GetService<ISettingsService>().RefreshSettings();
            //await ApplicationSettings.Instance.RefreshSettings();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await ApplicationSettings.Instance.SaveSettings();
        }
    }
}