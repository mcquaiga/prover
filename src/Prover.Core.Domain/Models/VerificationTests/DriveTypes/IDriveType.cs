namespace Prover.Domain.Models.VerificationTests.DriveTypes
{
    public interface IDriveType
    {
        string Discriminator { get; }
        bool HasPassed { get; }
        decimal UnCorrectedInputVolume(decimal appliedInput);
        int MaxUncorrectedPulses();
    }
}