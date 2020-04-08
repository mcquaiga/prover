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
        private readonly IDeviceSessionManager _deviceSessionManager;
        private readonly Func<EvcVerificationViewModel, IVolumeTestManager> _volumeTestManagerFactory;
        private readonly IActionsExecutioner _actionExecutioner;
        private readonly Func<EvcVerificationViewModel, IVolumeTestManager, ITestManager> _testManagerFactory;

        public VerificationTestManagerFactory(ILogger<VerificationTestManagerFactory> logger,
            IDeviceSessionManager deviceSessionManager,
            Func<EvcVerificationViewModel, IVolumeTestManager, ITestManager> testManagerFactory,
            Func<EvcVerificationViewModel, IVolumeTestManager> volumeTestManagerFactory,

            IActionsExecutioner actionExecutioner)
        {
            _logger = logger;
            _deviceSessionManager = deviceSessionManager;
            _testManagerFactory = testManagerFactory;
            _volumeTestManagerFactory = volumeTestManagerFactory;
            _actionExecutioner = actionExecutioner;
        }

        public async Task<ITestManager> StartNew(IVerificationTestService verificationService, DeviceType deviceType)
        {
            var device = await _deviceSessionManager.StartSession(deviceType);
            var testViewModel = verificationService.NewVerification(device);

            await _actionExecutioner.RunActionsOn<IInitializeAction>(testViewModel);

            await _deviceSessionManager.Disconnect();

            var volumeManager = _volumeTestManagerFactory.Invoke(testViewModel);
            return _testManagerFactory.Invoke(testViewModel, volumeManager);
        }
    }
}