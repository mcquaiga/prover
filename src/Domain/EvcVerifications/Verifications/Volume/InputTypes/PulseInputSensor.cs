using Devices.Core;
using Devices.Core.Interfaces;
using Domain.EvcVerifications.Builders;

namespace Domain.EvcVerifications.Verifications.Volume.InputTypes
{
    public class PulseInputSensor : IVolumeInputType
    {
        private readonly DeviceInstance _device;

        public PulseInputSensor(DeviceInstance device)
        {
            _device = device;
        }

        #region Public Properties

        public VolumeInputType InputType => VolumeInputType.PulseInput;

        #endregion

        #region Public Methods

        public int MaxUncorrectedPulses()
        {
            return 10;
        }

        public decimal UnCorrectedInputVolume(decimal appliedInput)
        {
            return 0m;
        }

        #endregion
    }
}