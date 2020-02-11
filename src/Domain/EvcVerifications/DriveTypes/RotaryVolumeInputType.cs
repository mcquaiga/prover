using System.Collections.Generic;
using Core.GasCalculations;
using Devices.Core;
using Devices.Core.Items;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;
using Domain.EvcVerifications.CorrectionTests;

namespace Domain.EvcVerifications.DriveTypes
{
    public class RotaryVolumeInputType : IVolumeInputType
    {
        public RotaryVolumeInputType(IVolumeItems volumeItems, IRotaryMeterItems rotaryItems)
        {
            VolumeItems = volumeItems;
            RotaryItems = rotaryItems;
        }

        #region Public Properties

        public IVolumeItems VolumeItems { get; }
        public IRotaryMeterItems RotaryItems { get; }

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
            return VolumeCalculator.Uncorrected(RotaryItems.MeterDisplacement, appliedInput);
        }

        #endregion
    }
}