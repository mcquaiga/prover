using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.VerificationTests
{
    abstract class VerificationBase
    {
        protected InstrumentCommunicator _instrumentCommunicator;
        protected Instrument _instrument;

        protected VerificationBase(Instrument instrument, InstrumentCommunicator instrumentComm)
        {
            _instrument = instrument;
            _instrumentCommunicator = instrumentComm;
        }
    }
}
