using System;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Communications;
using Client.Desktop.Wpf.Extensions;
using Client.Desktop.Wpf.ViewModels.Dialogs;
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
using Microsoft.Extensions.Logging;
using Prover.Application.Hardware;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Hardware.MccDAQ;
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
            services.AddTransient<TestManager>();
            services.AddTransient<ITestManagerViewModelFactory, TestManager>();

            //services.AddTransient<VolumeTestManager>();
            services.AddTransient<IVolumeTestManagerFactory, VolumeTestManagerFactory>();

            services.AddSingleton<IDeviceSessionManager, DeviceSessionManager>();

            // Port Setup
            var portFactory = new CommPortFactory();
            services.AddSingleton<ICommPortFactory>(c => portFactory);

            // Device Clients
            var clientFactory = new CommunicationsClientFactory(portFactory);
            services.AddSingleton<ICommClientFactory>(c => clientFactory);

            //Tachometer
            //services.AddScoped<ITachometerService, TachometerService>();

            /*
             * DAQ Board Setup 
             */

            // Simulator
            services.AddTransient<PulseOutputsListenerService>();
            //services.AddSingleton<Func<PulseOutputChannel, IInputChannel>>(c => channel => SimulatedInputChannel.PulseInputSimulators[channel]);
            //services.AddSingleton<Func<OutputChannelType, IOutputChannel>>(c => channel => SimulatedOutputChannel.OutputSimulators[channel]);

            services.AddTransient<DeviceSessionDialogManager>();

            //services.AddSingleton<DaqBoardChannelFactory>();
            //services.AddSingleton<IInputChannelFactory, DaqBoardChannelFactory>();
            //services.AddSingleton<IOutputChannelFactory, DaqBoardChannelFactory>();

            //Pulse Outputs
            services.AddSingleton(c => new SimulatorPulseChannelFactory(c.GetService<ILogger>()));
            services.AddSingleton<IInputChannelFactory, SimulatorPulseChannelFactory>();
            services.AddSingleton<IOutputChannelFactory, SimulatorPulseChannelFactory>();

            services.AddStartTask<DeviceServices>();
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _provider.GetService<IInputChannelFactory>();
            _provider.GetService<DaqBoardChannelFactory>();
           
            _provider.GetService<DeviceSessionDialogManager>();
           

            var repo = _provider.GetService<DeviceRepository>();
            await repo.Load(new[] {MiJsonDeviceTypeDataSource.Instance, RometJsonDeviceTypeDataSource.Instance});


        }
    }
}