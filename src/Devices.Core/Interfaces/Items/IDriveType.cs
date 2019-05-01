namespace Devices.Core.Interfaces.Items
{
    public interface IDriveType
    {
        #region Properties

        string Discriminator { get; }

        bool HasPassed { get; }

        #endregion

        #region Methods

        int MaxUncorrectedPulses();

        double? UnCorrectedInputVolume(double appliedInput);

        #endregion
    }

    public static class Drives
    {
        #region Fields

        public const string Mechanical = "Mechanical";

        public const string PulseInput = "Pulse Input";

        public const string Rotary = "Rotary";

        #endregion
    }
}