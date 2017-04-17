namespace Prover.InstrumentProtocol.Core.Messaging
{
    public abstract class ResponseCode
    {
        protected ResponseCode(int code)
        {
            Code = code;
        }

        public int Code { get; }
    }
}