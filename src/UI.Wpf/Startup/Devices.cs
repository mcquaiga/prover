using System;
using System.Threading;
using System.Threading.Tasks;
using Client.Wpf.Communications;
using Client.Wpf.ViewModels.Verifications;
using Devices;
using Devices.Communications.Interfaces;
using Devices.Communications.IO;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Repository.JsonRepository;
using Devices.Romet.Core.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MvvmDialogs;
using Shared.Interfaces;

namespace Client.Wpf.Startup
{
    internal class DeviceServices : IHostedService
    {
        private readonly IServiceProvider _provider;

        public DeviceServices(IServiceProvider provider)
        {
            _provider = provider;
        }

        public static void AddServices(IServiceCollection services, IConfiguration config)
        {
            services.AddHostedService<DeviceServices>();

            var portFactory = new CommPortFactory();
            var clientFactory = new CommunicationsClientFactory(portFactory);
            services.AddSingleton<ICommPortFactory>(c => portFactory);
            services.AddSingleton<ICommClientFactory>(c => clientFactory);

            services.AddTransient<DeviceSessionManager>();
            //(c => 
            //    new DeviceSessionManager(c.GetService<IDialogService>(), clientFactory, portFactory)
            //    );

            RegisterTypeDataSources(services);
        }

        private static void RegisterTypeDataSources(IServiceCollection services)
        {
            services.AddSingleton<IDeviceTypeDataSource<DeviceType>>(c => MiJsonDeviceTypeDataSource.Instance);
            services.AddSingleton<IDeviceTypeDataSource<DeviceType>>(c => RometJsonDeviceTypeDataSource.Instance);
            services.AddSingleton<IDeviceTypeCacheSource<DeviceType>>(c =>
                new KeyValueDeviceTypeDataSource(c.GetService<IKeyValueStore>()));

            services.AddSingleton(c => RepositoryFactory.Instance);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await RepositoryFactory.Create(
                _provider.GetService<IDeviceTypeCacheSource<DeviceType>>(), 
                _provider.GetServices<IDeviceTypeDataSource<DeviceType>>());   
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            
        }
    }
}