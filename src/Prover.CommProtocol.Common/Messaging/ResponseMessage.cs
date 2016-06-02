using NLog;

namespace Prover.CommProtocol.Common.Messaging
{
    public abstract class ResponseMessage
    {
        protected static Logger Log = LogManager.GetCurrentClassLogger();

        protected ResponseMessage(string checksum)
        {
            Checksum = checksum;
        }

        public string Checksum { get; protected set; }
    }
}