namespace Prover.Core.EVCTypes
{
    public interface IDriveType
    {
        decimal? UnCorrectedInputVolume(decimal appliedInput);
        string Discriminator { get; }
        int MaxUnCorrected();
        bool HasPassed { get; }
    }
}