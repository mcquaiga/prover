using Devices.Core.Items.Attributes;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;
using Devices.Honeywell.Core.Items.Attributes;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    public class VolumeItems : HoneywellItemGroup, IVolumeItems
    {
        [ItemInfo(90)]
        public decimal CorrectedMultiplier { get; protected set; }

        [JoinLowResHighResValue(0, 113)]
        public decimal CorrectedReading { get; protected set; }

        [ItemInfo(90)]
        public string CorrectedUnits { get; protected set; }

        [ItemInfo(98)]
        public decimal DriveRate { get; protected set; }

        [ItemInfo(98)]
        public string DriveRateDescription { get; protected set; }

        [ItemInfo(92)]
        public decimal UncorrectedMultiplier { get; protected set; }

        [JoinLowResHighResValue(2, 892)]
        public decimal UncorrectedReading { get; protected set; }

        [ItemInfo(92)]
        public string UncorrectedUnits { get; protected set; }
    }
}