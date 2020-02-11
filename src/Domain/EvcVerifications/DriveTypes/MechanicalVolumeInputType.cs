using System.Collections.Generic;
using System.Linq;
using Core.GasCalculations;
using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;
using Domain.EvcVerifications.CorrectionTests;

namespace Domain.EvcVerifications.DriveTypes
{
    public class MechanicalVolumeInputType : IVolumeInputType
    {
        public IVolumeItems VolumeItems { get; }
        public IEnumerable<VolumeInputTestSample> UncorrectedTestLimits { get; }

        public MechanicalVolumeInputType(IVolumeItems volumeItems, IEnumerable<VolumeInputTestSample> uncorrectedTestLimits = null)
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
            return VolumeCalculator.Uncorrected(VolumeItems.DriveRate, appliedInput);
        }
    }
}
