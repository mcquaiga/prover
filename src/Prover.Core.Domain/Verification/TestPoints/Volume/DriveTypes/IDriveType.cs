namespace Prover.Domain.Verification.TestPoints.Volume.DriveTypes
{
    public interface IDriveType
    {
        string Discriminator { get; }
        bool HasPassed { get; }
        int MaxUncorrectedPulses();
        double UnCorrectedInputVolume(double appliedInput);
    }
}