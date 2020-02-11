using System.Collections.Generic;
using System.Linq;
using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.Attributes;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Items.ItemGroups.Builders;
using Devices.Honeywell.Core.Items.Attributes;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    //public class VolumeCorrectedItems : HoneywellItemGroup, IVolumeCorrectedItems
    //{
    //    [ItemInfo(90)]
    //    public decimal CorrectedMultiplier { get; set; }

    //    [JoinLowResHighResValue(0, 113)]
    //    public decimal CorrectedReading { get; set; }

    //    [ItemInfo(90)]
    //    public string CorrectedUnits { get; set; }
    //}

    public class VolumeItems : HoneywellItemGroup, IVolumeItems
    {
        public VolumeInputType VolumeInputType { get; set; }

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

    public class VolumeItemGroupBuilder : ItemGroupBuilderBase<VolumeItems>, IBuildItemsFor<VolumeItems>
    {
        public VolumeItemGroupBuilder(DeviceType deviceType) : base(deviceType)
        {
        }

        public virtual VolumeItems Build(DeviceType device, IEnumerable<ItemValue> values)
        {
            var items = values.ToList();

            var volume = GetItemGroupInstance(typeof(VolumeItems), items);

            volume.DriveRateDescription = items.GetItemDescription(98).Description;

            volume.VolumeInputType = volume.DriveRateDescription.Contains("Rotary") 
                ? VolumeInputType.Rotary 
                : VolumeInputType.Mechanical;

            volume.CorrectedReading = ItemValueParses.JoinLowResHighResReading(items.GetItemValue(0), items.GetItemValue(113));
            volume.CorrectedMultiplier = items.GetItemValue(90);
            volume.CorrectedUnits = items.GetItem<ItemValueWithDescription>(90).GetDescription();

            volume.UncorrectedReading = ItemValueParses.JoinLowResHighResReading(items.GetItemValue(2), items.GetItemValueNullable(892) ?? 0);
            volume.UncorrectedMultiplier = items.GetItemValue(92);
            volume.UncorrectedUnits = items.GetItemDescription(92).Description;

            return volume;
        }
    }
}