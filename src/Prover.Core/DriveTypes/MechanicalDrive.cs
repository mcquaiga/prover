using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;

namespace Prover.Core.DriveTypes
{
    public class MechanicalDrive : IDriveType
    {
        public MechanicalDrive(Instrument instrument)
        {
            Instrument = instrument;
        }

        public Instrument Instrument { get; }

        public string Discriminator => "Mechanical";

        public bool HasPassed => true;

        public int MaxUncorrectedPulses() => 100;

        public decimal? UnCorrectedInputVolume(decimal appliedInput)
        {
            return appliedInput*Instrument.DriveRate();
        }
    }
}