namespace Prover.Domain.Models.TestRuns.DriveTypes
{
    public interface IDriveType
    {
        bool HasPassed { get; }
        decimal UnCorrectedInputVolume(decimal appliedInput);
        int MaxUncorrectedPulses();
    } 
}