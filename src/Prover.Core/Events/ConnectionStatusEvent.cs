namespace Prover.Core.Events
{
    public class ConnectionStatusEvent
    {
        public enum Status
        {
            Connecting,
            Connected,
            Disconnected
        }

        public ConnectionStatusEvent(Status status, int attemptCount = 0, int maxAttempts = 10)
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