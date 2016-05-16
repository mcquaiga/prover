using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Events
{
    public class CommunicationStatusEvent
    {
        public CommunicationStatusEvent(string statusMessage)
        {
            Message = statusMessage;
        }

        public string Message { get; private set; }
    }
}
