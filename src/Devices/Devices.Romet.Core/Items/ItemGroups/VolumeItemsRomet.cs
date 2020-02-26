using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Honeywell.Core.Items.ItemGroups;
using Prover.Shared;

namespace Devices.Romet.Core.Items.ItemGroups
{
    public class VolumeItemsRomet : VolumeItemsHoneywell
    {
        private const int HighResUncorItemNumber = 767;
        private const int MeterSizeItemNumber = 768;
        private const int UncorrectedItemNumber = 2;
        private const int HighResCorItemNumber = 113;
        private const int CorrectedItemNumber = 0;

        public override ItemGroup SetValues(DeviceType deviceType, IEnumerable<ItemValue> itemValues)
        {
            var items = itemValues.ToList();

            base.SetValues(deviceType,  items);

            var volume = this;

            this.VolumeInputType = !string.IsNullOrEmpty(items.GetItem(MeterSizeItemNumber).GetValue().ToString())
                ? VolumeInputType.Rotary
                : VolumeInputType.Mechanical;

            this.CorrectedReading =
                JoinLowResHighResReading(
                    items.GetItemValue(CorrectedItemNumber),
                    items.GetItemValue(HighResCorItemNumber));

            this.UncorrectedReading =
                JoinLowResHighResReading(
                    items.GetItemValue(UncorrectedItemNumber),
                    items.GetItemValueNullable(HighResUncorItemNumber) ?? 0);

            return this;
        }


        private decimal JoinLowResHighResReading(decimal? lowResValue, decimal? highResValue)
        {
            if (!lowResValue.HasValue || !highResValue.HasValue)
                throw new ArgumentNullException(nameof(lowResValue) + " & " + nameof(highResValue));

            var result = $"{lowResValue}.{highResValue.ToString().Replace(lowResValue.ToString(), string.Empty)}";

            return decimal.Parse(result);
        }
    }

}