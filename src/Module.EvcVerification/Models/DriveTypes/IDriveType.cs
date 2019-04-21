namespace Module.EvcVerification.Models.DriveTypes
{
    public static class Drives
    {
        public const string PulseInput = "Pulse Input";
        public const string Mechanical = "Mechanical";
        public const string Rotary = "Rotary";
    }

    public interface IDriveType
    {        
        string Discriminator { get; }
        bool HasPassed { get; }
        double? UnCorrectedInputVolume(double appliedInput);
        int MaxUncorrectedPulses();
        Energy Energy { get; }
    }
}