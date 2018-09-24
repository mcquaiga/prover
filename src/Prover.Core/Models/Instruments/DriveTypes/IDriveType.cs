namespace Prover.Core.Models.Instruments.DriveTypes
{
    public static class DriveTypes
    {
        public const string PulseInput = "Pulse Input";
        public const string Mechanical = "Mechanical";
        public const string Rotary = "Rotary";
    }

    public interface IDriveType
    {
        string Discriminator { get; }
        bool HasPassed { get; }
        decimal? UnCorrectedInputVolume(decimal appliedInput);
        int MaxUncorrectedPulses();
        Energy Energy { get; }
    }
}