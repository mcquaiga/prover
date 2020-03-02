using System.Collections.Generic;
using System.Linq;
using Core.GasCalculations;
using Devices.Core.Items.ItemGroups;
using Prover.Shared;

namespace Prover.Domain.EvcVerifications.Verifications.Volume.InputTypes
{
    public class MechanicalVolumeInputType : IVolumeInputType
    {
        protected MechanicalVolumeInputType()
        {

        }

        public VolumeItems VolumeItems { get; }
        public IEnumerable<VolumeInputTestSample> UncorrectedTestLimits { get; }

        public MechanicalVolumeInputType(VolumeItems volumeItems, IEnumerable<VolumeInputTestSample> uncorrectedTestLimits = null)
        {
            VolumeItems = volumeItems;
            UncorrectedTestLimits = uncorrectedTestLimits;
        }

        public int MaxUncorrectedPulses()
        {
            return UncorrectedTestLimits?.FirstOrDefault(x => x.CuFtValue == VolumeItems.UncorrectedMultiplier)?.UncorrectedPulseTarget 
                   ?? 10;
        }

        public VolumeInputType InputType => VolumeInputType.Mechanical;

        public decimal UnCorrectedInputVolume(decimal appliedInput)
        {
            return VolumeCalculator.TrueUncorrected(VolumeItems.DriveRate, appliedInput);
        }

    }
}
