using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Items.ItemGroups.Builders;
using Devices.Honeywell.Core.Items.ItemGroups;

namespace Devices.Romet.Core.Items.ItemGroups
{
    internal class VolumeItemGroupBuilderRomet : VolumeItemGroupBuilder, IBuildItemsFor<VolumeItems>
    {
        private const int HighResUncorItemNumber = 767;
        private const int MeterSizeItemNumber = 768;
        private const int UncorrectedItemNumber = 2;
        private const int HighResCorItemNumber = 113;
        private const int CorrectedItemNumber = 0;

        #region Public Methods

        public VolumeItemGroupBuilderRomet(DeviceType deviceType) : base(deviceType)
        {
        }

        public override VolumeItems Build(DeviceType device, IEnumerable<ItemValue> values)
        {
            var items = values.ToList();
            var volume = base.Build(device, items);

            volume.VolumeInputType = !string.IsNullOrEmpty(items.GetItem(MeterSizeItemNumber).GetValue().ToString())
                ? VolumeInputType.Rotary
                : VolumeInputType.Mechanical;

            volume.CorrectedReading =
                JoinLowResHighResReading(
                    items.GetItemValue(CorrectedItemNumber),
                    items.GetItemValue(HighResCorItemNumber));

            volume.UncorrectedReading =
                JoinLowResHighResReading(
                    items.GetItemValue(UncorrectedItemNumber),
                    items.GetItemValueNullable(HighResUncorItemNumber) ?? 0);

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
}