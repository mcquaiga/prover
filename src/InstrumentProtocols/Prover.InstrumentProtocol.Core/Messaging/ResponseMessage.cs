using NLog;

namespace Prover.InstrumentProtocol.Core.Messaging
{
    public abstract class ResponseMessage
    {

        protected ResponseMessage()
        {

        }

        protected static Logger Log = LogManager.GetCurrentClassLogger();
    }
}