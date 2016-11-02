namespace Prover.CommProtocol.Common.Messaging
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