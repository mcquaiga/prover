using Prover.Core.EVCTypes;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;

namespace Prover.Core.DriveTypes
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

        public int MaxUncorrectedPulses() => 100;

        public decimal? UnCorrectedInputVolume(decimal appliedInput)
        {
            return appliedInput*Instrument.DriveRate();
        }
    }
}
