using Devices.Communications.IO;
using Devices.Core.Interfaces;
using System;

namespace Devices.Communications
{
    public class FailedConnectionException : Exception
    {
        #region Constructors

        public FailedConnectionException(ICommPort commPort, IDevice evcType, int retryAttempts, Exception reason = null)
            : base($"{commPort.Name} Could not connect to {evcType.Name} after {retryAttempts} retries.", reason)
        {
        }

        private FailedConnectionException() : base()
        {
        }

        private FailedConnectionException(string message) : base(message)
        {
        }

        private FailedConnectionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion
    }
}