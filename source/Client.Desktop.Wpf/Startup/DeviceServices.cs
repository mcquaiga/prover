using System;
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
            services.AddTransient<TestManagerViewModel>();
            services.AddTransient<ITestManagerViewModelFactory, TestManagerViewModel>();

            services.AddTransient<VolumeTestManager>();
            services.AddTransient<IVolumeTestManagerFactory, VolumeTestManagerFactory>();

            services.AddScoped<DeviceSessionManager>();

            // Port Setup
            var portFactory = new CommPortFactory();
            services.AddSingleton<ICommPortFactory>(c => portFactory);

            // Device Clients
            var clientFactory = new CommunicationsClientFactory(portFactory);
            services.AddSingleton<ICommClientFactory>(c => clientFactory);

            //Tachometer


            /*
             * DAQ Board Setup 
             */

            // Simulator
            services.AddTransient<PulseInputsListenerService>();
            services.AddSingleton<Func<PulseOutputChannel, IInputChannel>>(c => channel => SimulatedInputChannel.PulseInputSimulators[channel]);
            services.AddSingleton<Func<OutputChannelType, IOutputChannel>>(c => channel => SimulatedOutputChannel.OutputSimulators[channel]);

            //services.AddSingleton<DaqBoardChannelFactory>();
            //services.AddSingleton<IInputChannelFactory, DaqBoardChannelFactory>();
            //services.AddSingleton<IOutputChannelFactory, DaqBoardChannelFactory>();

            //Pulse Outputs
            //services.AddTransient(c => new PulseInputsListenerService(c))
            //services.AddSingleton<IInputChannelFactory>(c => new SimulatorPulseChannelFactory());


            services.AddStartTask<DeviceServices>();
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var repo = _provider.GetService<DeviceRepository>();
            await repo.Load(new[] {MiJsonDeviceTypeDataSource.Instance, RometJsonDeviceTypeDataSource.Instance});

            _provider.GetService<IInputChannelFactory>();
        }
    }
}