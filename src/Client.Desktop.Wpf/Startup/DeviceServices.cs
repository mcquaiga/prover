using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Services;
using Application.ViewModels.Services;
using Client.Wpf.Communications;
using Client.Wpf.Extensions;
using Devices;
using Devices.Communications.Interfaces;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Repository.JsonRepository;
using Devices.Romet.Core.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Interfaces;

namespace Client.Wpf.Startup
{
    internal class DeviceServices : IHaveStartupTask
    {
        private readonly IServiceProvider _provider;

        public DeviceServices(IServiceProvider provider) => _provider = provider;

        #region IHaveStartupTask Members

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await RepositoryFactory.Create(
                _provider.GetService<IDeviceTypeCacheSource<DeviceType>>(),
                _provider.GetServices<IDeviceTypeDataSource<DeviceType>>());
        }

        #endregion

        public static void AddServices(IServiceCollection services, HostBuilderContext host)
        {
            var portFactory = new CommPortFactory();
            var clientFactory = new CommunicationsClientFactory(portFactory);
            services.AddSingleton<ICommPortFactory>(c => portFactory);
            services.AddSingleton<ICommClientFactory>(c => clientFactory);

            services.AddTransient<VerificationTestManager>();
            services.AddTransient<DeviceSessionManager>();

            services.AddScoped<VerificationViewModelService>();

            RegisterTypeDataSources(services);

            services.AddStartTask<DeviceServices>();
        }

        private static void RegisterTypeDataSources(IServiceCollection services)
        {
            services.AddSingleton<IDeviceTypeDataSource<DeviceType>>(c => MiJsonDeviceTypeDataSource.Instance);
            services.AddSingleton<IDeviceTypeDataSource<DeviceType>>(c => RometJsonDeviceTypeDataSource.Instance);
            services.AddSingleton<IDeviceTypeCacheSource<DeviceType>>(c =>
                new KeyValueDeviceTypeDataSource(c.GetService<IKeyValueStore>()));

            services.AddSingleton(c => RepositoryFactory.Instance);
        }
    }
}