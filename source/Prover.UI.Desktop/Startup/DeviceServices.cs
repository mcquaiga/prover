using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prover.Application.Interfaces;
using Prover.Application.Verifications;
using Prover.Application.ViewModels.Factories;
using Prover.Shared.Interfaces;
using Prover.UI.Desktop.Extensions;
using Prover.UI.Desktop.ViewModels.Devices;

namespace Prover.UI.Desktop.Startup
{
    internal class DeviceServices : IHostedService
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

            services.AddHostedService<DeviceServices>();
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

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _provider.GetService<IInputChannelFactory>();
            //_provider.GetService<DaqBoardChannelFactory>();

            _provider.GetService<DeviceSessionDialogManager>();

            InitializeEventSubscribers();

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}