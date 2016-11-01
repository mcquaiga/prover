using Prover.Core.Models.Instruments;

namespace Prover.Core.DriveTypes
{
    public interface IDriveType
    {
        string Discriminator { get; }
        bool HasPassed { get; }
        decimal? UnCorrectedInputVolume(Instrument instrument, decimal appliedInput);
        int MaxUncorrectedPulses(Instrument instrument);
    }
}