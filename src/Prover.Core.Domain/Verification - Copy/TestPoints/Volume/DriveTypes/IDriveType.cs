namespace Prover.Domain.Verification.TestPoints.Volume.DriveTypes
{
    public interface IDriveType
    {
        int MaxUncorrectedPulses();
        double UnCorrectedInputVolume(double appliedInput);
    }
}