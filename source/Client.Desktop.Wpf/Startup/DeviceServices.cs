using Client.Desktop.Wpf.Communications;
using Client.Desktop.Wpf.Extensions;
using Client.Desktop.Wpf.ViewModels.Dialogs;
using Devices.Communications.Interfaces;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Repository.JsonRepository;
using Devices.Romet.Core.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prover.Application.Hardware;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using System;
using Prover.Application.VerificationManager;
using Prover.Application.VerificationManager.Volume;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Volume.Factories;
using Prover.Shared;
using Prover.Shared.Interfaces;

using System;

using System.Threading;
using System.Threading.Tasks;

namespace Client.Desktop.Wpf.Startup
{
    internal static class DeviceServiceEx
    {
        public static void AddDeviceCommunication(this IServiceCollection services)
        {
            services.AddSingleton<IDeviceSessionManager, DeviceSessionManager>();
            // Port Setup
            var portFactory = new CommPortFactory();
            var clientFactory = new CommunicationsClientFactory(portFactory);

            // Device Clients
            //services.AddSingleton<ICommPortFactory>(c => portFactory);
            //services.AddSingleton<ICommClientFactory>(c => clientFactory);
            services.AddSingleton<Func<DeviceType, ICommunicationsClient>>(c => (device) =>
            {
                var port = portFactory.Create(ApplicationSettings.Local.InstrumentCommPort,
                    ApplicationSettings.Local.InstrumentBaudRate);

                return clientFactory.Create(device, port);
            });
        }

        public static void AddPulseOutputListeners(this IServiceCollection services)
        {
            services.AddTransient<PulseOutputsListenerService>();
            services.AddSingleton<Func<PulseOutputsListenerService>>(c => c.GetService<PulseOutputsListenerService>);

            services.AddSingleton<Func<PulseOutputChannel, IInputChannel>>(c => channel =>
            {
                return SimulatedInputChannel.PulseInputSimulators[channel];
            });
            services.AddSingleton<Func<OutputChannelType, IOutputChannel>>(c => channel =>
            {
                return SimulatedOutputChannel.OutputSimulators[channel];
            });

            //services.AddSingleton<DaqBoardChannelFactory>();
            //services.AddSingleton<IInputChannelFactory, DaqBoardChannelFactory>();
            //services.AddSingleton<IOutputChannelFactory, DaqBoardChannelFactory>();

            // Pulse Outputs
            // Simulator
            //services.AddSingleton(c => new SimulatorPulseChannelFactory(c.GetService<ILogger>()));
            //services.AddSingleton<IInputChannelFactory, SimulatorPulseChannelFactory>();
            //services.AddSingleton<IOutputChannelFactory, SimulatorPulseChannelFactory>();
        }

        public static void AddTachometer(this IServiceCollection services)
        {
            services.AddSingleton<Func<IAppliedInputVolume>>(c =>
                () =>
                {
                    if (ApplicationSettings.Local.TachIsNotUsed) return new NullTachometerService();

                    return new TachometerService(
                        c.GetService<ILogger<TachometerService>>(),
                        ApplicationSettings.Local.TachCommPort,
                        c.GetService<Func<OutputChannelType, IOutputChannel>>().Invoke(OutputChannelType.Tachometer)
                    );
                });
        }
    }

    internal class DeviceServices : IHaveStartupTask
    {
        public DeviceServices(IServiceProvider provider) => _provider = provider;

        public static void AddServices(IServiceCollection services, HostBuilderContext host)
        {
            services.AddSingleton<IVerificationViewModelFactory, VerificationViewModelFactory>();

            services.AddSingleton<Func<EvcVerificationViewModel, VerificationTestService, ITestManager>>(c =>
                (test, service) => new TestManager(
                    c.GetService<ILogger<TestManager>>(),
                    c.GetService<IDeviceSessionManager>(),
                    test,
                    c.GetService<Func<EvcVerificationViewModel, IVolumeTestManager>>()));

            //services.AddTransient<ITestManagerFactory, TestManager>();

            services.AddSingleton<IVolumeTestManagerFactory, VolumeTestManagerFactory>();
            services.AddSingleton<Func<EvcVerificationViewModel, IVolumeTestManager>>(c => (evcTest) =>
                {
                    var volumeFactory = c.GetService<IVolumeTestManagerFactory>();
                    return volumeFactory.CreateVolumeManager(evcTest);
                });

            services.AddDeviceCommunication();
            services.AddPulseOutputListeners();
            services.AddTachometer();

            services.AddSingleton<DeviceSessionDialogManager>();

            services.AddStartTask<DeviceServices>();
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _provider.GetService<IInputChannelFactory>();
            //_provider.GetService<DaqBoardChannelFactory>();

            _provider.GetService<DeviceSessionDialogManager>();

            var repo = _provider.GetService<DeviceRepository>();
            await repo.Load(new[] { MiJsonDeviceTypeDataSource.Instance, RometJsonDeviceTypeDataSource.Instance });
        }

        private readonly IServiceProvider _provider;
    }
}