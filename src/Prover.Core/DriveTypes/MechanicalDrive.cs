using Prover.Core.Models.Instruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.EVCTypes
{
    public class MechanicalDrive : IDriveType
    {
        public MechanicalDrive(Instrument instrument)
        {
            this.Instrument = instrument;
        }

        public string Discriminator
        {
            get
            {
                return "Mechanical";
            }
        }

        public bool HasPassed
        {
            get
            {
                return false;
            }
        }

        public Instrument Instrument { get; private set; }

        public int MaxUnCorrected()
        {
            throw new NotImplementedException();
        }

        public decimal? UnCorrectedInputVolume(decimal appliedInput)
        {
            // AppliedInput * DriveRate
            throw new NotImplementedException();
        }
    }
}
