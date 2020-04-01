using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interfaces;

namespace Prover.Application.VerificationManager
{
    public class DeviceVerificationValidator : IDeviceVerificationValidator
    {
        private readonly IDeviceSessionManager _deviceSessionManager;
        private readonly IEnumerable<IDeviceValidation> _deviceValidations;
        private readonly ILogger<DeviceVerificationValidator> _logger;

        public DeviceVerificationValidator(IDeviceSessionManager deviceSessionManager, IEnumerable<IDeviceValidation> deviceValidations, ILogger<DeviceVerificationValidator> logger = null)
        {
            _deviceSessionManager = deviceSessionManager;
            _deviceValidations = deviceValidations;
            _logger = logger ?? NullLogger<DeviceVerificationValidator>.Instance;
        }

        public async Task RunValidations(DeviceInstance device)
        {
            await _deviceSessionManager.Connect();
            foreach (var deviceValidation in _deviceValidations)
            {
                var isValid = await deviceValidation.Validate(_deviceSessionManager, device);
            }

            await _deviceSessionManager.Disconnect();
        }
    }
}