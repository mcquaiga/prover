namespace Prover.Core.Events
{
    public class CommunicationStatusEvent
    {
        public CommunicationStatusEvent(string statusMessage)
        {
            Message = statusMessage;
        }

        public string Message { get; private set; }
    }
}