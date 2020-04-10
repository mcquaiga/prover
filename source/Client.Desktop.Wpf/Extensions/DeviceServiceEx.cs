using System;
using Client.Desktop.Wpf.Communications;
using Devices.Communications.Interfaces;
using Devices.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prover.Application.FileLoader;
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
            //services.AddSingleton<IActiveDeviceSessionManager>(c => c.GetRequiredService<DeviceSessionManager>());
            
            //services.AddSingleton<FileDeviceSessionManager>();
            //services.AddSingleton<IActiveDeviceSessionManager>(c => c.GetRequiredService<FileDeviceSessionManager>());

            //services.AddSingleton<IDeviceSessionManager>(c =>
            //{
            //    var managers = c.GetServices<IActiveDeviceSessionManager>();
            //    return managers.FirstOrDefault(m => m.Active);
            //});

            //services.AddSingleton<Func<Type, IDeviceSessionManager>>(c => (managerType) =>
            //{
            //    var manager = (IDeviceSessionManager) c.GetService(managerType);
                
            //    return manager;
            //});

            // Port Setup
            var portFactory = new CommPortFactory();
            var clientFactory = new CommunicationsClientFactory(portFactory);

            services.AddSingleton<FileCommunicationsClient>();

            // Device Clients
            services.AddSingleton<Func<DeviceType, ICommunicationsClient>>(c => (device) =>
            {
                if (!string.IsNullOrEmpty(ApplicationSettings.Local.VerificationFilePath))
                {
                    var fileComm = c.GetRequiredService<FileCommunicationsClient>();
                    fileComm.SetFilePath(ApplicationSettings.Local.VerificationFilePath);
                    return fileComm;
                }

                var port = portFactory.Create(
                    ApplicationSettings.Local.InstrumentCommPort,
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