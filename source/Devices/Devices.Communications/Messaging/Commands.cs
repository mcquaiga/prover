namespace Devices.Communications.Messaging
{
    public abstract class CommandDefinition<TResponse>
        where TResponse : ResponseMessage
    {
        #region Public Properties

        public string Command { get; set; }

        public abstract ResponseProcessor<TResponse> ResponseProcessor { get; }

        #endregion Public Properties
    }
}