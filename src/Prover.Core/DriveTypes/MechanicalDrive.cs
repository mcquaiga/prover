using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;

namespace Prover.Core.DriveTypes
{
    public class MechanicalDrive : IDriveType
    {
        public MechanicalDrive()
        {
        }

        public string Discriminator => "Mechanical";

        public bool HasPassed => true;

        public Instrument Instrument { get; private set; }

        public int MaxUncorrectedPulses(Instrument instrument) => 100;

        public decimal? UnCorrectedInputVolume(Instrument instrument, decimal appliedInput)
        {
            return appliedInput*instrument.DriveRate();
        }
    }
}
