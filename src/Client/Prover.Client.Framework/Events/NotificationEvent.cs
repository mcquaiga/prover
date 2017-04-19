namespace Prover.Client.Framework.Events
{
    public class NotificationEvent
    {
        public NotificationEvent(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}