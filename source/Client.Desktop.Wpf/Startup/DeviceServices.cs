using Client.Desktop.Wpf.Extensions;
using Devices.Core.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Volume.Factories;
using Prover.Shared.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.Wpf.ViewModels.Devices;
using Prover.Application.Verifications;
using Prover.Application.Verifications.Volume;

namespace Client.Desktop.Wpf.Startup
{
    internal class DeviceServices : IStartupTask
    {
        public DeviceServices(IServiceProvider provider) => _provider = provider;

        public static void AddServices(IServiceCollection services, HostBuilderContext host)
        {
            services.AddSingleton<IVerificationViewModelFactory, VerificationViewModelFactory>();

            services.AddSingleton<Func<EvcVerificationViewModel, IVolumeTestManager, ITestManager>>(c =>
                (test, volumeManager) => new TestManager(
                    c.GetService<ILogger<TestManager>>(),
                    c.GetService<IDeviceSessionManager>(),
                    test,
                    volumeManager,
                    c.GetService<IVerificationActionsExecutioner>()));

            //services.AddSingleton<Func<DeviceType, ITestManager>>(c => (deviceType) =>
            //{
            //    var session = c.GetService<IDeviceSessionManager>();
            //    var device = await session.StartSession(deviceType);
            //    var test = c.GetService<VerificationTestService>().NewTest(device);

            //    return new TestManager(new TestManager(
            //        c.GetService<ILogger<TestManager>>(),
            //        c.GetService<IDeviceSessionManager>(),
            //        test,
            //        c.GetService<Func<EvcVerificationViewModel, IVolumeTestManager>>(),
            //        c.GetService<IDeviceVerificationValidator>());
            //});

            services.AddTransient<ITestManagerFactory, VerificationTestManagerFactory>();

            services.AddSingleton<IVolumeTestManagerFactory, VolumeTestManagerFactory>();
            services.AddSingleton<Func<EvcVerificationViewModel, IVolumeTestManager>>(c => (evcTest) =>
                {
                    var volumeFactory = c.GetService<IVolumeTestManagerFactory>();
                    return volumeFactory.CreateVolumeManager(evcTest);
                });

            services.AddDeviceCommunication();
            services.AddPulseOutputListeners();
            services.AddTachometer();

            services.AddSingleton<IDeviceRepository, DeviceRepository>();
            services.AddSingleton<IVerificationActionsExecutioner, VerificationCustomActionsExecutioner>();
            services.AddSingleton<DeviceSessionDialogManager>();

            services.AddStartTask<DeviceServices>();
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _provider.GetService<IInputChannelFactory>();
            //_provider.GetService<DaqBoardChannelFactory>();

            _provider.GetService<DeviceSessionDialogManager>();
        }

        private readonly IServiceProvider _provider;
    }
}