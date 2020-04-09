using System;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Prover.Application.Interfaces;
using Prover.Application.VerificationManagers.Corrections;
using Prover.Application.ViewModels;

namespace Prover.Application.VerificationManagers.Factories
{
    public class VerificationTestManagerFactory : ITestManagerFactory
    {
        private readonly ILogger<VerificationTestManagerFactory> _logger;

        private readonly IVerificationTestService _verificationService;
        private readonly Func<EvcVerificationViewModel, IVolumeTestManager> _volumeTestManagerFactory;
        private readonly Func<EvcVerificationViewModel, ICorrectionTestsManager> _correctionsRunnerFactory;
        private readonly IActionsExecutioner _actionExecutioner;
        private readonly IDeviceSessionManager _deviceManager;
        private readonly Func<EvcVerificationViewModel, IVolumeTestManager, ITestManager> _testManagerFactory;

        public VerificationTestManagerFactory(
            ILogger<VerificationTestManagerFactory> logger,
            IVerificationTestService verificationService,
            IActionsExecutioner actionExecutioner,
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
            _actionExecutioner = actionExecutioner;
            _deviceManager = deviceManager;
        }

        public async Task<ITestManager> StartNew(DeviceType deviceType)
        {
            var device = await _deviceManager.StartSession(deviceType);
            var testViewModel = _verificationService.NewVerification(device);

            await _actionExecutioner.RunActionsOn<IOnInitializeAction>(testViewModel);

            await _deviceManager.Disconnect();

            var volumeManager = _volumeTestManagerFactory.Invoke(testViewModel);
            return _testManagerFactory.Invoke(testViewModel, volumeManager);
        }
    }

    public class VerificationManagerBuilder
    {

    }
}