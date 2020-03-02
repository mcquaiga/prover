using Devices.Core.Interfaces;
using System;
using Prover.Shared.IO;

namespace Devices.Communications
{
    public class FailedConnectionException : Exception
    {
        public FailedConnectionException(ICommPort commPort, DeviceType evcType, int retryAttempts, Exception reason = null)
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
    }
}