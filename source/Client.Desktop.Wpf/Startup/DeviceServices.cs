using Client.Desktop.Wpf.Extensions;
using Client.Desktop.Wpf.ViewModels.Dialogs;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Repository.JsonRepository;
using Devices.Romet.Core.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using System;
using Prover.Application.VerificationManager;
using Prover.Application.VerificationManager.Volume;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Volume.Factories;
using Prover.Shared.Interfaces;

using System;

using System.Threading;
using System.Threading.Tasks;
using Devices.Core.Interfaces;

namespace Client.Desktop.Wpf.Startup
{
    internal class DeviceServices : IHaveStartupTask
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
                    c.GetService<IDeviceVerificationValidator>()));

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

            services.AddSingleton<IDeviceVerificationValidator, DeviceVerificationValidator>();
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