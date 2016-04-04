using Prover.Core.Models.Instruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.GUI.ViewModels.InstrumentsList.PTZVerification
{
    public class PTZVerificationSetViewModel
    {
        public PTZVerificationSetViewModel(Instrument.VerificationTest verificationTest)
        {
            this.VerificationTest = verificationTest;
        }

        public Instrument.VerificationTest VerificationTest { get; private set; }
    }
}
 