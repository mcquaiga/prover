using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Communications;
using Client.Desktop.Wpf.Extensions;
using Client.Desktop.Wpf.ViewModels.Verifications;
using Devices;
using Devices.Communications.Interfaces;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Repository.JsonRepository;
using Devices.Romet.Core.Repository;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prover.Application.Hardware;
using Prover.Application.Services;
using Prover.Infrastructure.KeyValueStore;
using Prover.Shared;
using Prover.Shared.Interfaces;

namespace Client.Desktop.Wpf.Startup
{
    internal class DeviceServices : IHaveStartupTask
    {
        private readonly IServiceProvider _provider;

        public DeviceServices(IServiceProvider provider) => _provider = provider;

        public static void AddServices(IServiceCollection services, HostBuilderContext host)
        {
            RegisterTypeDataSources(services);

            services.AddTransient<TestManagerViewModel>();
            services.AddTransient<ITestManagerViewModelFactory, TestManagerViewModel>();

            services.AddTransient<VolumeTestManager>();
            services.AddTransient<IVolumeTestManagerFactory, VolumeTestManagerFactory>();

            services.AddTransient<DeviceSessionManager>();
            services.AddScoped<VerificationViewModelService>();

            // Port Setup
            var portFactory = new CommPortFactory();
            services.AddSingleton<ICommPortFactory>(c => portFactory);

            // Device Clients
            var clientFactory = new CommunicationsClientFactory(portFactory);
            services.AddSingleton<ICommClientFactory>(c => clientFactory);

            //Tachometer

            //Pulse Outputs
            //services.AddTransient(c => new PulseInputsListenerService(c))

            //services.AddSingleton<IInputChannelFactory>(c => new SimulatorPulseChannelFactory());

            services.AddTransient<PulseInputsListenerService>();


            //=> new PulseInputsListenerService(c.GetService<ILoggerFactory>(), , null));
            services.AddSingleton<Func<PulseOutputChannel, IInputChannel>>(c => channel => SimulatedInputChannel.PulseInputSimulators[channel]);
            services.AddSingleton<Func<OutputChannelType, IOutputChannel>>(c => channel => SimulatedOutputChannel.OutputSimulators[channel]);

            /*
             * DAQ Board Setup 
             */
            //services.AddSingleton<DaqBoardChannelFactory>();
            //services.AddSingleton<IInputChannelFactory, DaqBoardChannelFactory>();
            //services.AddSingleton<IOutputChannelFactory, DaqBoardChannelFactory>();


            services.AddStartTask<DeviceServices>();
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var repo = await RepositoryFactory.Create(
                _provider.GetService<IDeviceTypeCacheSource<DeviceType>>(),
                _provider.GetServices<IDeviceTypeDataSource<DeviceType>>());


            _provider.GetService<IInputChannelFactory>();
        }

        private static void RegisterTypeDataSources(IServiceCollection services)
        {
            services.AddSingleton<IDeviceTypeDataSource<DeviceType>>(c => MiJsonDeviceTypeDataSource.Instance);
            services.AddSingleton<IDeviceTypeDataSource<DeviceType>>(c => RometJsonDeviceTypeDataSource.Instance);

            services.AddScoped<IRepository<DeviceType>>(c =>
                new LiteDbRepository<DeviceType>(c.GetService<ILiteDatabase>()));
            services.AddSingleton<IDeviceTypeCacheSource<DeviceType>, DeviceTypeCacheSource>();
            //services.AddSingleton<IDeviceTypeCacheSource<DeviceType>>(c =>
            //    new LiteDbRepository<DeviceType>(c.GetService<ILiteDatabase>()));

            services.AddSingleton(c => RepositoryFactory.Instance);
        }
    }
}