using System;
using System.Runtime.Serialization;

namespace Devices.Communications.Messaging
{
    [Serializable]
    public class EvcResponseException : Exception
    {
        #region Constructors

        public EvcResponseException() : base()
        {
        }

        public EvcResponseException(string message) : base(message)
        {
        }

        public EvcResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EvcResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion
    }
}