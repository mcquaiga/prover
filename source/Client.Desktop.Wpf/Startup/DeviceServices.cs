using System;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Extensions;
using Client.Desktop.Wpf.ViewModels;
using Client.Desktop.Wpf.ViewModels.Devices;
using Client.Desktop.Wpf.ViewModels.Verifications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using Prover.Shared.Interfaces;
using Prover.Application.Verifications;
using Prover.Application.Verifications.Corrections;
using Prover.Application.Verifications.Factories;
using Prover.Application.Verifications.Volume;
using Prover.Application.ViewModels.Factories;

namespace Client.Desktop.Wpf.Startup
{
    internal class DeviceServices : IStartupTask
    {
        private readonly ILogger<DeviceServices> _logger;
        private readonly IServiceProvider _provider;

        public DeviceServices(ILogger<DeviceServices> logger, IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        public static void AddServices(IServiceCollection services, HostBuilderContext host)
        {
            services.AddSingleton<IVerificationViewModelFactory, VerificationViewModelFactory>();
            
            services.AddVerificationManagers();
            services.AddDeviceCommunication();
            services.AddPulseOutputListeners();
            services.AddTachometer();

            services.AddSingleton<DeviceSessionDialogManager>();
            //services.AddSingleton<IActionsExecutioner, VerificationActionsExecutor>();

            services.AddAllTypes<IEventsSubscriber>(lifetime: ServiceLifetime.Singleton);

            services.AddStartTask<DeviceServices>();
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _provider.GetService<IInputChannelFactory>();
            //_provider.GetService<DaqBoardChannelFactory>();

            _provider.GetService<DeviceSessionDialogManager>();

            InitializeEventSubscribers();

            await Task.CompletedTask;
        }

        private void InitializeEventSubscribers()
        {
            VerificationEvents.DefaultSubscribers();
            _logger.LogDebug("Verification Event subscribers:");

            var eventSubscribers = _provider.GetServices<IEventsSubscriber>();
            eventSubscribers.ForEach(e =>
            {
                _logger.LogDebug($"   Type: {e}");
                e.SubscribeToEvents();
            });
        }
    }
}