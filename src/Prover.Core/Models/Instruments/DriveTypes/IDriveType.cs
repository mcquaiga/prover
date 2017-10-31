namespace Prover.Core.Models.Instruments.DriveTypes
{
    public interface IDriveType
    {
        string Discriminator { get; }
        bool HasPassed { get; }
        decimal? UnCorrectedInputVolume(decimal appliedInput);
        int MaxUncorrectedPulses();
    }
}