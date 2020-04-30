using Devices.Honeywell.Comm.Messaging.Responses;
using System;

namespace Devices.Honeywell.Comm.Exceptions
{
    [Serializable]
    public class SignOnErrorException : HoneywellResponseException
    {
        #region Constructors

        public SignOnErrorException(StatusResponseMessage responseMessage)
            : base(responseMessage)
        {
        }

        #endregion
    }
}