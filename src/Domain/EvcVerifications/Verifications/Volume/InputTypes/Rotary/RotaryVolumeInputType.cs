using Core.GasCalculations;
using Devices.Core;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;

namespace Domain.EvcVerifications.Verifications.Volume.InputTypes.Rotary
{
    public class RotaryVolumeInputType : IVolumeInputType
    {
        public RotaryVolumeInputType(VolumeItems volumeItems, RotaryMeterItems rotaryItems)
        {
            VolumeItems = volumeItems;
            RotaryItems = rotaryItems;
        }

        #region Public Properties

        public VolumeItems VolumeItems { get; }
        public RotaryMeterItems RotaryItems { get; }

        #endregion

        #region Public Methods

        public int MaxUncorrectedPulses()
        {
            if (VolumeItems.UncorrectedMultiplier == 10)
                return RotaryItems.MeterType.UnCorPulsesX10;

            if (VolumeItems.UncorrectedMultiplier == 100)
                return RotaryItems.MeterType.UnCorPulsesX100;

            return 10; //Low standard number if we can't find anything
        }

        public VolumeInputType InputType => VolumeInputType.Rotary;

        public decimal UnCorrectedInputVolume(decimal appliedInput)
        {
            return VolumeCalculator.TrueUncorrected(RotaryItems.MeterDisplacement, appliedInput);
        }

        #endregion
    }
}