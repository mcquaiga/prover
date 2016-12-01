using Prover.Core.Models.Instruments;

namespace Prover.Core.DriveTypes
{
    public interface IDriveType
    {
        string Discriminator { get; }
        bool HasPassed { get; }
        decimal? UnCorrectedInputVolume(decimal appliedInput);
        int MaxUncorrectedPulses();
    }
}