using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Items.ItemGroups.Builders;
using Shared;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    public class PressureItemsBuilder : ItemGroupBuilderBase<PressureItems>, IBuildItemsFor<PressureItems>
    {
        #region Public Methods

        public PressureItemsBuilder(DeviceType deviceType) : base(deviceType)
        {
        }

        public PressureItems Build(DeviceType device, IEnumerable<ItemValue> values)
        {
            var items = values.ToList();

            var site = device.GetGroupValues<global::Devices.Core.Items.ItemGroups.SiteInformationItems>(items);
            if (site.PressureFactor == CorrectionFactorType.Fixed) return default;

            var p = new PressureItemsHoneywell();
            return SetValues(p, items);
        }

        #endregion
    }
}