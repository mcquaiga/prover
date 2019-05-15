using Devices.Communications.Messaging;
using Devices.Honeywell.Comm.Messaging.Responses;
using System;

namespace Devices.Honeywell.Comm.Exceptions
{
    public class HoneywellResponseException : EvcResponseException
    {
        #region Constructors

        public HoneywellResponseException(StatusResponseMessage responseMessage) : base()
        {
            ResponseMessage = responseMessage;
        }

        private HoneywellResponseException() : base()
        {
        }

        private HoneywellResponseException(string message) : base(message)
        {
        }

        private HoneywellResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion

        #region Properties

        public ResponseCode Code => ResponseMessage.ResponseCode;

        public StatusResponseMessage ResponseMessage { get; }

        #endregion
    }
}