using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Events
{
    public class InstrumentConnectionEvent
    {
        public enum Status
        {
            Connecting,
            Connected,
            Disconnected
        }

        public InstrumentConnectionEvent(Status status, int attemptCount = 0, int maxAttempts = 10)
        {
            ConnectionStatus = status;
            AttemptCount = attemptCount;
            MaxAttempts = maxAttempts;
        }

        public Status ConnectionStatus { get; private set; }
        public int AttemptCount { get; private set; }
        public int MaxAttempts { get; private set; }
    }
}
