using Devices.Communications.Messaging;
using Devices.Honeywell.Comm.CommClients;
using System;

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

        public HoneywellResponse(ResponseCode code, Action<BaseHoneywellClient> recoveryAction) : this(code)
        {
            RecoveryAction = recoveryAction;
        }

        public ResponseCode Code { get; }

        public bool ThrowsException => _exceptionFactory != null;

        protected virtual Action<BaseHoneywellClient> RecoveryAction { get; } = _ => { };

        public EvcResponseException RaiseException(StatusResponseMessage response)
                            => _exceptionFactory.Invoke(response);

        public virtual void TryRecover(BaseHoneywellClient client)
        {
            RecoveryAction.Invoke(client);
        }
    }
}