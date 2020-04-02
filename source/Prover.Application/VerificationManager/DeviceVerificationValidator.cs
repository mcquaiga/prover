using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;

namespace Prover.Application.VerificationManager
{
    public class VerificationCustomActionsExecutioner : IVerificationActionsExecutioner
    {
        private readonly IDeviceSessionManager _deviceSessionManager;
        private readonly IEnumerable<IVerificationCustomActions> _verificationActions;
        private readonly ILogger<VerificationCustomActionsExecutioner> _logger;

        public VerificationCustomActionsExecutioner(IDeviceSessionManager deviceSessionManager, IEnumerable<IVerificationCustomActions> verificationActions, ILogger<VerificationCustomActionsExecutioner> logger = null)
        {
            _deviceSessionManager = deviceSessionManager;
            _verificationActions = verificationActions;
            _logger = logger ?? NullLogger<VerificationCustomActionsExecutioner>.Instance;
        }

        public async Task RunCustomActions(VerificationTestStep testStep, EvcVerificationViewModel verificationTest, DeviceInstance device)
        {
            await _deviceSessionManager.Connect();
            foreach (var action in _verificationActions.Where(v => v.RunOnStep == testStep))
            {
                var isValid = await action.Run(_deviceSessionManager, device, verificationTest);
            }

            await _deviceSessionManager.Disconnect();
        }

    }
}