using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Prover.Application.Interfaces;
using Prover.Application.Verifications.Corrections;
using Prover.Application.ViewModels;

namespace Prover.Application.Verifications.Factories
{
    public static class ManagerFactoryMixins
    {
        public static async Task<IDeviceQaTestManager> CreateTestManager(this DeviceInstance deviceInstance)
        {
            return await VerificationManagerFactory.CreateManager(deviceInstance.DeviceType);
        }
    }

    public class VerificationManagerFactory : IVerificationManagerFactory
    {
        private static IDeviceSessionManager _deviceManager;
        private static ILogger<VerificationManagerFactory> _logger;
        private static Func<EvcVerificationViewModel, IDeviceQaTestManager> _testManagerFactory;
        private static IVerificationTestService _verificationService;
        
        public VerificationManagerFactory(ILogger<VerificationManagerFactory> logger,
                IVerificationTestService verificationService,
                IDeviceSessionManager deviceManager,
                Func<EvcVerificationViewModel, IDeviceQaTestManager> testManagerFactory)
        {
            _logger = logger;
            _verificationService = verificationService;
            _testManagerFactory = testManagerFactory;
            _deviceManager = deviceManager;
        }

        public async Task<IDeviceQaTestManager> StartNew(DeviceType deviceType)
        {
            return await CreateManager(deviceType);
        }

        public static async Task<IDeviceQaTestManager> CreateManager(DeviceType deviceType)
        {
            if (_deviceManager == null) throw new NullReferenceException(nameof(VerificationManagerFactory));

            var device = await _deviceManager.StartSession(deviceType);
            var testViewModel = _verificationService.NewVerification(device);

            await VerificationEvents.OnInitialize.Publish(testViewModel);

            await _deviceManager.Disconnect();

            return _testManagerFactory.Invoke(testViewModel);
        }
    }
}