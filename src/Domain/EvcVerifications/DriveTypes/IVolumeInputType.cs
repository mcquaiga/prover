using Devices.Core;

namespace Domain.EvcVerifications.DriveTypes
{
    public interface IVolumeInputType
    {
        #region Public Properties

        VolumeInputType InputType { get; }

        #endregion

        #region Public Methods

        int MaxUncorrectedPulses();

        decimal UnCorrectedInputVolume(decimal appliedInput);

        #endregion
    }
}