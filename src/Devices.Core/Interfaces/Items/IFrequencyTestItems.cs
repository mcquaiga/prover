namespace Devices.Core.Interfaces.Items
{
    public interface IFrequencyTestItems
    {
        #region Public Properties

        long MainAdjustedVolumeReading { get; set; }
        long MainUnadjustVolumeReading { get; set; }
        decimal TibAdjustedVolumeReading { get; set; }
        long TibUnadjustedVolumeReading { get; }

        #endregion Public Properties
    }
}