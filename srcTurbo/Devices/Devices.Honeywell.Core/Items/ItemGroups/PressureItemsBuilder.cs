using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Items.ItemGroups.Builders;
using Devices.Honeywell.Core.Items.ItemGroups.Builders;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    public class PressureItemsBuilder : ItemGroupBuilderBase<PressureItems>, IBuildItemsFor<PressureItems>
    {
        #region Public Methods

        public PressureItems Build(DeviceType device, IEnumerable<ItemValue> values)
        {
            var items = values.ToList();

            var site = device.GetGroupValues<ISiteInformationItems>(items);
            if (site.PressureFactorLive == CorrectionFactorType.Fixed) return default;

            var p = new PressureItems();
            return SetValues(p, items);
        }

        #endregion
    }
}