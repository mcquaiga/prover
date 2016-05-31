using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.CommProtocol.Common.CommPorts;

namespace Prover.CommProtocol.MiHoneywell.CommClients
{
    public class MiniMaxClient : HoneywellClientBase
    {
        public MiniMaxClient(CommPort commPort) : base(commPort, InstrumentType.MiniMax)
        {
        }
    }
}
