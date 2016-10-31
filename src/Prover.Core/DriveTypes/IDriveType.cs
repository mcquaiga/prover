using Prover.Core.Models.Instruments;

namespace Prover.Core.DriveTypes
{

    public interface IDriveType
    {
        decimal? UnCorrectedInputVolume(Instrument instrument, decimal appliedInput);
        string Discriminator { get; }
        int MaxUncorrectedPulses(Instrument instrument);
        bool HasPassed { get; }
    }
}