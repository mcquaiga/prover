using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Prover.Core.VerificationTests;
using Prover.Core.VerificationTests.Rotary;

namespace Prover.GUI.Events
{
    public class InstrumentUpdateEvent
    {
        public RotaryTestManager InstrumentManager { get; set; }

        public InstrumentUpdateEvent(RotaryTestManager instrumentManager)
        {
            InstrumentManager = instrumentManager;
        } 
    }
}
