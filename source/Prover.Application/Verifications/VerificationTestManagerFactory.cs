using System;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;

namespace Prover.Application.Verifications
{
    public class VerificationTestManagerFactory : ITestManagerFactory
    {
        private readonly ILogger<VerificationTestManagerFactory> _logger;

        private readonly IVerificationTestService _verificationService;
        private readonly Func<EvcVerificationViewModel, IVolumeTestManager> _volumeTestManagerFactory;
        private readonly IActionsExecutioner _actionExecutioner;
        private readonly Func<EvcVerificationViewModel, IVolumeTestManager, ITestManager> _testManagerFactory;

        public VerificationTestManagerFactory(
            ILogger<VerificationTestManagerFactory> logger,
            IVerificationTestService verificationService,
            Func<EvcVerificationViewModel, IVolumeTestManager, ITestManager> testManagerFactory,
            Func<EvcVerificationViewModel, IVolumeTestManager> volumeTestManagerFactory,
            Func<EvcVerificationViewModel, ICorrectionVerificationRunner> correctionsRunnerFactory,
            IActionsExecutioner actionExecutioner)
        {
            _logger = logger;
          
            _verificationService = verificationService;
            _testManagerFactory = testManagerFactory;
            _volumeTestManagerFactory = volumeTestManagerFactory;
            _actionExecutioner = actionExecutioner;
        }

        public async Task<ITestManager> StartNew(IDeviceSessionManager deviceManager, DeviceType deviceType)
        {
            var device = await deviceManager.StartSession(deviceType);
            var testViewModel = _verificationService.NewVerification(device);

            await _actionExecutioner.RunActionsOn<IOnInitializeAction>(testViewModel);

            await deviceManager.Disconnect();

            var volumeManager = _volumeTestManagerFactory.Invoke(testViewModel);
            return _testManagerFactory.Invoke(testViewModel, volumeManager);
        }
    }
}