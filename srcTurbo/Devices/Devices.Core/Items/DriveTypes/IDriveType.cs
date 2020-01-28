namespace Devices.Core.Items.DriveTypes
{
    public interface IDriveType
    {
        string Discriminator { get; }

        int MaxUncorrectedPulses();

        decimal UnCorrectedInputVolume(decimal appliedInput);
    }

    public static class Drives
    {
        public const string Mechanical = "Mechanical";

        public const string PulseInput = "Pulse Input";

        public const string Rotary = "Rotary";
    }
}