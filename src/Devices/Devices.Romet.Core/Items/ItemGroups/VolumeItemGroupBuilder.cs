using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Items.ItemGroups.Builders;
using Devices.Honeywell.Core.Items.ItemGroups;

namespace Devices.Romet.Core.Items.ItemGroups
{
    internal class VolumeItemGroupBuilder : ItemGroupBuilderBase<VolumeItems>, IBuildItemsFor<VolumeItems>
    {
        #region Public Methods

        public VolumeItems Build(DeviceType device, IEnumerable<ItemValue> values)
        {
            var items = values.ToList();

            var volume = GetItemGroupInstance(typeof(VolumeItems), items);

            volume.DriveRateDescription = items.GetItemDescription(98).Description;

            volume.CorrectedReading = JoinLowResHighResReading(items.GetItemValue(0),
                items.GetItemValue(113));

            volume.UncorrectedReading = JoinLowResHighResReading(items.GetItemValue(2), 0);

            volume.CorrectedMultiplier = items.GetItem<ItemValueWithDescription>(90).GetValue();
            volume.CorrectedUnits = items.GetItem<ItemValueWithDescription>(90).GetDescription();

            volume.UncorrectedMultiplier = items.GetItemValue(92);

            volume.UncorrectedUnits = items.GetItemDescription(92).Description;

            return volume;
        }

        #endregion

        #region Private

        private decimal JoinLowResHighResReading(decimal? lowResValue, decimal? highResValue)
        {
            if (!lowResValue.HasValue || !highResValue.HasValue)
                throw new ArgumentNullException(nameof(lowResValue) + " & " + nameof(highResValue));

            var result = $"{lowResValue}.{highResValue.ToString().Replace(lowResValue.ToString(), string.Empty)}";

            return decimal.Parse(result);
        }

        #endregion
    }

    internal class SiteInformationItemsRomet : SiteInformationItems, ISiteInformationItems
    {
    }
}