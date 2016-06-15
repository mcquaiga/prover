namespace Prover.GUI.Common.Events
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