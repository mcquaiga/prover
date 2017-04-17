namespace Prover.InstrumentProtocol.Core.Messaging
{
    public abstract class CommandDefinition<TResponse>
        where TResponse : ResponseMessage
    {
        public string Command { get; set; }
        public abstract ResponseProcessor<TResponse> ResponseProcessor { get; }
    }
}