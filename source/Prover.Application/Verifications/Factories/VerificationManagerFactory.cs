using Devices.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Prover.Application.Interfaces;
using Prover.Application.Mappers;
using Prover.Application.Models.EvcVerifications.Builders;
using Prover.Application.ViewModels;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Prover.Application.Verifications.Factories
{
    public static class ManagerFactoryMixins
    {
        public static async Task<IQaTestRunManager> CreateTestManager(this DeviceInstance deviceInstance)
        {
            return await VerificationManagerFactory.CreateManager(deviceInstance.DeviceType);
        }
    }

    public class VerificationManagerFactory : IVerificationManagerFactory
    {
        private static IDeviceSessionManager _deviceManager;
        private static ILogger<VerificationManagerFactory> _logger;
        private static Func<EvcVerificationViewModel, IQaTestRunManager> _testManagerFactory;
        private static IVerificationTestService _verificationService;

        public VerificationManagerFactory(ILogger<VerificationManagerFactory> logger,
                IVerificationTestService verificationService,
                IDeviceSessionManager deviceManager,
                Func<EvcVerificationViewModel, IQaTestRunManager> testManagerFactory)
        {
            _logger = logger;
            _verificationService = verificationService;
            _testManagerFactory = testManagerFactory;
            _deviceManager = deviceManager;
        }

        public async Task<IQaTestRunManager> StartNew(DeviceType deviceType)
        {
            return await CreateManager(deviceType);
        }

        public static async Task<IQaTestRunManager> CreateManager(DeviceType deviceType)
        {
            if (_deviceManager == null)
                throw new NullReferenceException(nameof(VerificationManagerFactory));

            var device = await _deviceManager.StartSession(deviceType);
            var testViewModel = device.NewVerification().ToViewModel();

            await VerificationEvents.OnInitialize.Publish(testViewModel);

            await _deviceManager.Disconnect();

            return _testManagerFactory.Invoke(testViewModel);
        }
    }
}