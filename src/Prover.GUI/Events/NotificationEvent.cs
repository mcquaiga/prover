namespace Prover.GUI.Events
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