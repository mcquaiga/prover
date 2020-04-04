using System;
using Client.Desktop.Wpf.Communications;
using Devices.Communications.Interfaces;
using Devices.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prover.Application.Hardware;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Shared;
using Prover.Shared.Interfaces;

namespace Client.Desktop.Wpf.Extensions
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
}