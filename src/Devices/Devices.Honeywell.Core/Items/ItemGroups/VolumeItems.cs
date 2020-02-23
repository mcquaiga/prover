using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.Attributes;
using Devices.Core.Items.ItemGroups;
using Devices.Honeywell.Core.Items.Attributes;
using Shared;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    //public class VolumeCorrectedItems : HoneywellItemGroup, VolumeItems
    //{
    //    [ItemInfo(90)]
    //    public decimal CorrectedMultiplier { get; set; }

    //    [JoinLowResHighResValue(0, 113)]
    //    public decimal CorrectedReading { get; set; }

    //    [ItemInfo(90)]
    //    public string CorrectedUnits { get; set; }
    //}

    public class VolumeItemsHoneywell : VolumeItems
    {
        public override VolumeInputType VolumeInputType { get; set; }

        [ItemInfo(90)]
        public override decimal CorrectedMultiplier { get; set; }

        [JoinLowResHighResValue(0, 113)]
        public override decimal CorrectedReading { get; set; }

        [ItemInfo(90)]
        public override string CorrectedUnits { get; set; }

        [ItemInfo(98)]
        public override decimal DriveRate { get; set; }

        [ItemInfo(98)]
        public override string DriveRateDescription { get; set; }

        [ItemInfo(92)]
        public override decimal UncorrectedMultiplier { get; set; }

        [JoinLowResHighResValue(2, 892)]
        public override decimal UncorrectedReading { get; set; }

        [ItemInfo(92)]
        public override string UncorrectedUnits { get; set; }

        public override ItemGroup SetValues(DeviceType deviceType, IEnumerable<ItemValue> itemValues)
        {
            var items = itemValues.ToList();

            base.SetValues(deviceType, items);

            this.DriveRateDescription = items.GetItemDescription(98).Description;

            this.VolumeInputType = this.DriveRateDescription.Contains("Rotary") 
                ? VolumeInputType.Rotary 
                : VolumeInputType.Mechanical;

            this.CorrectedReading = ItemValueParses.JoinLowResHighResReading(items.GetItemValue(0), items.GetItemValue(113));
            this.CorrectedMultiplier = items.GetItemValue(90);
            this.CorrectedUnits = items.GetItem<ItemValueWithDescription>(90).GetDescription();

            this.UncorrectedReading = ItemValueParses.JoinLowResHighResReading(items.GetItemValue(2), items.GetItemValueNullable(892) ?? 0);
            this.UncorrectedMultiplier = items.GetItemValue(92);
            this.UncorrectedUnits = items.GetItemDescription(92).Description;

            return this;
        }
    }

}