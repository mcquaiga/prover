using Prover.Core.Models.Instruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Core.Extensions;

namespace Prover.Core.EVCTypes
{
    public class MechanicalDrive : IDriveType
    {
        public MechanicalDrive(Instrument instrument)
        {
            this.Instrument = instrument;
        }

        public string Discriminator => "Mechanical";

        public bool HasPassed => true;

        public Instrument Instrument { get; private set; }

        public int MaxUnCorrected()
        {
            return 100;
        }

        public decimal? UnCorrectedInputVolume(decimal appliedInput)
        {
            return appliedInput*Instrument.DriveRate();
        }
    }
}
