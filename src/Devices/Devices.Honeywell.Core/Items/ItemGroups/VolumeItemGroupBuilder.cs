using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups.Builders;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    public class VolumeItemGroupBuilder : ItemGroupBuilderBase<VolumeItems>, IBuildItemsFor<VolumeItems>
    {
        public VolumeItems Build(DeviceType device, IEnumerable<ItemValue> values)
        {
            var items = values.ToList();

            var volume = GetItemGroupInstance(typeof(VolumeItems), items);

            volume.DriveRateDescription = items.GetItemDescription(98).Description;

            volume.CorrectedReading = ItemValueParses.JoinLowResHighResReading(items.GetItemValue(0), items.GetItemValue(113));
            volume.CorrectedMultiplier = items.GetItem<ItemValueWithDescription>(90).GetValue();
            volume.CorrectedUnits = items.GetItem<ItemValueWithDescription>(90).GetDescription();

            volume.UncorrectedReading = ItemValueParses.JoinLowResHighResReading(items.GetItemValue(2), items.GetItemValue(892));
            volume.UncorrectedMultiplier = items.GetItemValue(92);
            volume.UncorrectedUnits = items.GetItemDescription(92).Description;

            return volume;
        }
    }
}