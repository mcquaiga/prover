using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.CommProtocol.Messages.Events
{
    public interface IEvent { }

    public class ConnectingToInstrumentEvent
    {
        public string PortInfo
        {
            get; private set;
        }

        public ConnectingToInstrumentEvent(string portInfo)
        {
            this.PortInfo = portInfo;
        }
    }

}
