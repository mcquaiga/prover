using Devices.Communications.Messaging;
using Devices.Honeywell.Comm.CommClients;
using System;

namespace Devices.Honeywell.Comm.Messaging.Responses.Codes
{
    internal class HoneywellResponse
    {
        #region Constructors

        public HoneywellResponse(ResponseCode code, Func<StatusResponseMessage, EvcResponseException> exceptionFactory)
        {
            Code = code;
            _exceptionFactory = exceptionFactory;
        }

        public HoneywellResponse(ResponseCode code)
        {
            Code = code;
        }

        public HoneywellResponse(ResponseCode code, Action<HoneywellClient> recoveryAction) : this(code)
        {
            RecoveryAction = recoveryAction;
        }

        #endregion

        #region Properties

        public ResponseCode Code { get; }

        public bool ThrowsException => _exceptionFactory != null;

        #endregion

        #region Methods

        public EvcResponseException RaiseException(StatusResponseMessage response)
                    => _exceptionFactory.Invoke(response);

        public virtual void TryRecover(HoneywellClient client)
        {
            RecoveryAction.Invoke(client);
        }

        protected virtual Action<HoneywellClient> RecoveryAction { get; } = _ => { };

        #endregion

        #region Fields

        private Func<StatusResponseMessage, EvcResponseException> _exceptionFactory;

        #endregion
    }
}