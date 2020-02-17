using Devices.Communications.Messaging;
using Devices.Honeywell.Comm.CommClients;
using System;
using Devices.Core.Interfaces;
using Devices.Honeywell.Core;

namespace Devices.Honeywell.Comm.Messaging.Responses.Codes
{
    internal class HoneywellResponse
    {
        private Func<StatusResponseMessage, EvcResponseException> _exceptionFactory;

        public HoneywellResponse(ResponseCode code, Func<StatusResponseMessage, EvcResponseException> exceptionFactory)
        {
            Code = code;
            _exceptionFactory = exceptionFactory;
        }

        public HoneywellResponse(ResponseCode code)
        {
            Code = code;
        }

        public HoneywellResponse(ResponseCode code, Action<HoneywellClientBase<HoneywellDeviceType>> recoveryAction) : this(code)
        {
            RecoveryAction = recoveryAction;
        }

        public ResponseCode Code { get; }

        public bool ThrowsException => _exceptionFactory != null;

        protected virtual Action<HoneywellClientBase<HoneywellDeviceType>> RecoveryAction { get; } = _ => { };

        public EvcResponseException RaiseException(StatusResponseMessage response)
                            => _exceptionFactory.Invoke(response);

        public virtual void TryRecover(HoneywellClientBase<HoneywellDeviceType> client)
        {
            RecoveryAction.Invoke(client);
        }
    }
}