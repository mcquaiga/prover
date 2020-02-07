using Devices.Core.Items.Attributes;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;
using Devices.Honeywell.Core.Items.Attributes;
using Devices.Honeywell.Core.Items.ItemGroups.Builders;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    public class VolumeItems : HoneywellItemGroup, IVolumeItems
    {
        [ItemInfo(90)]
        public decimal CorrectedMultiplier { get; set; }

        [JoinLowResHighResValue(0, 113)]
        public decimal CorrectedReading { get; set; }

        [ItemInfo(90)]
        public string CorrectedUnits { get; set; }

        [ItemInfo(98)]
        public decimal DriveRate { get; set; }

        [ItemInfo(98)]
        public string DriveRateDescription { get; set; }

        [ItemInfo(92)]
        public decimal UncorrectedMultiplier { get; set; }

        [JoinLowResHighResValue(2, 892)]
        public decimal UncorrectedReading { get; set; }

        [ItemInfo(92)]
        public string UncorrectedUnits { get; set; }
    }
}