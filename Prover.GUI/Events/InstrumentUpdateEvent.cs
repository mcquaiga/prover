using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Core.Models.Instruments;

namespace Prover.GUI.Events
{
    public class InstrumentUpdateEvent
    {
        public Instrument Instrument { get; set; }

        public InstrumentUpdateEvent(Instrument instrument)
        {
            Instrument = instrument;
        }

        public Core.Communication.InstrumentManager InstrumentManager { get; set; }
    }
}
