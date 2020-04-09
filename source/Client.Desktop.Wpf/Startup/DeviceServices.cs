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
using Client.Desktop.Wpf.ViewModels;
using Client.Desktop.Wpf.ViewModels.Devices;
using Prover.Application.VerificationManagers;
using Prover.Application.VerificationManagers.Corrections;
using Prover.Application.VerificationManagers.Factories;

namespace Client.Desktop.Wpf.Startup
{
    internal class DeviceServices : IStartupTask
    {
        public DeviceServices(IServiceProvider provider) => _provider = provider;

        public static void AddServices(IServiceCollection services, HostBuilderContext host)
        {
            services.AddSingleton<IVerificationViewModelFactory, VerificationViewModelFactory>();

            services.AddSingleton<Func<EvcVerificationViewModel, IVolumeTestManager, ITestManager>>(c =>
                (test, volumeManager) => ActivatorUtilities.CreateInstance<RotaryTestManager>(c, test, volumeManager));
                    //new TestManager(
                    //c.GetService<ILogger<TestManager>>(),
                    //c.GetService<IDeviceSessionManager>(),
                    //test,
                    //volumeManager,
                    //c.GetService<IActionsExecutioner>()));

            services.AddTransient<ITestManagerFactory, VerificationTestManagerFactory>();
            services.AddTransient<TestManagerFactoryCoordinator>();
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
            services.AddSingleton<DeviceSessionDialogManager>();

            services.AddSingleton<IActionsExecutioner, VerificationActionsExecutor>();
            services.AddSingleton<ICorrectionTestsManager, StabilizerCorrectionTestManager>();

            services.AddStartTask<DeviceServices>();
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _provider.GetService<IInputChannelFactory>();
            //_provider.GetService<DaqBoardChannelFactory>();

            _provider.GetService<DeviceSessionDialogManager>();
            await Task.CompletedTask;
        }

        private readonly IServiceProvider _provider;
    }
}