using Devices.Core;
using Devices.Core.Interfaces.Items;
using Devices.Honeywell.Core.Attributes;

namespace Devices.Honeywell.Core.ItemGroups
{
    internal class VolumeItems : ItemGroupBase, IVolumeItems
    {
        [ItemInfo(90)]
        public decimal CorrectedMultiplier { get; protected set; }

        [JoinLowResHighResValueAttribute(0, 113)]
        public decimal CorrectedReading { get; protected set; }

        [ItemInfo(90)]
        public string CorrectedUnits { get; protected set; }

        [ItemInfo(98)]
        public decimal DriveRate { get; protected set; }

        [ItemInfo(98)]
        public string DriveRateDescription { get; protected set; }

        public IDriveType DriveType { get; protected set; }

        [ItemInfo(92)]
        public decimal UncorrectedMultiplier { get; protected set; }

        [JoinLowResHighResValueAttribute(2, 892)]
        public decimal UncorrectedReading { get; protected set; }

        [ItemInfo(92)]
        public string UncorrectedUnits { get; protected set; }
    }
}