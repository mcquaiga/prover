using Core.GasCalculations;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;
using Prover.Shared;

namespace Prover.Domain.EvcVerifications.Verifications.Volume.InputTypes.Rotary
{
    public class RotaryVolumeInputType : IVolumeInputType
    {
        private RotaryVolumeInputType()
        {
        }

        public RotaryVolumeInputType(VolumeItems volumeItems, RotaryMeterItems rotaryItems)
        {
            VolumeItems = volumeItems;
            RotaryItems = rotaryItems;

            MeterDisplacement = RotaryItems.MeterType?.MeterDisplacement ??
                                (RotaryItems.MeterDisplacement != 0 ? RotaryItems.MeterDisplacement : -1);
        }

        public RotaryVolumeInputType(RotaryMeterItems rotaryItems) => RotaryItems = rotaryItems;

        public decimal MeterDisplacement { get; }

        public VolumeItems VolumeItems { get; set; }
        public RotaryMeterItems RotaryItems { get; set; }

        public VolumeInputType InputType => VolumeInputType.Rotary;

        public int MaxUncorrectedPulses()
        {
            if (VolumeItems.UncorrectedMultiplier == 10)
                return RotaryItems.MeterType.UnCorPulsesX10;

            if (VolumeItems.UncorrectedMultiplier == 100)
                return RotaryItems.MeterType.UnCorPulsesX100;

            return 10; //Low standard number if we can't find anything
        }

        public decimal UnCorrectedInputVolume(decimal appliedInput) =>
            VolumeCalculator.TrueUncorrected(MeterDisplacement, appliedInput);
    }
}