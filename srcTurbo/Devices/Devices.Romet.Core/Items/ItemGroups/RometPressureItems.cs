using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Items.ItemGroups.Builders;
using Devices.Honeywell.Core;
using Devices.Honeywell.Core.Items;
using Devices.Honeywell.Core.Items.ItemGroups;

namespace Devices.Romet.Core.Items.ItemGroups
{
    internal class VolumeItemGroupBuilderRomet : VolumeItemGroupBuilder
    {
        public VolumeItemGroupBuilderRomet(RometDeviceType deviceType) : base(deviceType)
        {
        }

        public override VolumeItems Build<T>(DeviceType device, IEnumerable<ItemValue> values)
        {
            return Build(device, values);
        }

        public override VolumeItems Build(DeviceType device, IEnumerable<ItemValue> values)
        {
            var items = values.ToList();

            var volume = base.Build(device, items);

            volume.DriveRateDescription = items.GetItemDescription(98).Description;

            volume.CorrectedReading = ItemValueParses.JoinLowResHighResReading(items.GetItemValue(0),
                items.GetItemValue(113));

            volume.UncorrectedReading = ItemValueParses.JoinLowResHighResReading(items.GetItemValue(2), 0);

            volume.CorrectedMultiplier = items.GetItem<ItemValueWithDescription>(90).GetValue();
            volume.CorrectedUnits = items.GetItem<ItemValueWithDescription>(90).GetDescription();

            volume.UncorrectedMultiplier = items.GetItemValue(92);
         
            volume.UncorrectedUnits = items.GetItemDescription(92).Description;

            return volume;
        }
    }


}
