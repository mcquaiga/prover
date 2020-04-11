using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Prover.Application.Interfaces;
using Prover.Application.Verifications.Corrections;
using Prover.Application.Verifications.Events;
using Prover.Application.ViewModels;

namespace Prover.Application.Verifications.Factories
{
    public class VerificationTestManagerFactory : ITestManagerFactory
    {
        private readonly ILogger<VerificationTestManagerFactory> _logger;

        private readonly IVerificationTestService _verificationService;
        private readonly IEnumerable<IEventsSubscriber> _verificationEventsSubscribers;
        private readonly Func<EvcVerificationViewModel, IVolumeTestManager> _volumeTestManagerFactory;
        private readonly Func<EvcVerificationViewModel, ICorrectionTestsManager> _correctionsRunnerFactory;
        private readonly IDeviceSessionManager _deviceManager;
        private readonly Func<EvcVerificationViewModel, IVolumeTestManager, ITestManager> _testManagerFactory;

        public VerificationTestManagerFactory(
            ILogger<VerificationTestManagerFactory> logger,
            IVerificationTestService verificationService,
            IDeviceSessionManager deviceManager,
            Func<EvcVerificationViewModel, IVolumeTestManager, ITestManager> testManagerFactory,
            Func<EvcVerificationViewModel, IVolumeTestManager> volumeTestManagerFactory,
            Func<EvcVerificationViewModel, ICorrectionTestsManager> correctionsRunnerFactory = null)
        {
            _logger = logger;
          
            _verificationService = verificationService;
            _testManagerFactory = testManagerFactory;
            _volumeTestManagerFactory = volumeTestManagerFactory;
            _correctionsRunnerFactory = correctionsRunnerFactory ?? (test => new StabilizerCorrectionTestManager(deviceManager));
           
            _deviceManager = deviceManager;
        }

        public async Task<ITestManager> StartNew(DeviceType deviceType)
        {
            var device = await _deviceManager.StartSession(deviceType);
            var testViewModel = _verificationService.NewVerification(device);

            await VerificationEvents.OnInitialize.Publish(testViewModel);

            await _deviceManager.Disconnect();

            var volumeManager = _volumeTestManagerFactory.Invoke(testViewModel);
            return _testManagerFactory.Invoke(testViewModel, volumeManager);
        }
    }

    public class VerificationManagerBuilder
    {

    }
}