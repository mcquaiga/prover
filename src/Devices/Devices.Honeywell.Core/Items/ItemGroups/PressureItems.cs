using System.Collections.Generic;
using System.Linq;
using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.Attributes;
using Devices.Core.Items.ItemGroups;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    internal class PressureItemsHoneywell : PressureItems
    {
        #region Public Properties

        [ItemInfo(14)] public override decimal AtmosphericPressure { get; set; }

        [ItemInfo(13)] public override decimal Base { get; set; }

        [ItemInfo(44)] public override decimal Factor { get; set; }

        [ItemInfo(8)] public override decimal GasPressure { get; set; }

        [ItemInfo(137)] public override int Range { get; set; }

        [ItemInfo(112)] public override PressureTransducerType TransducerType { get; set; }

        [ItemInfo(87)] public override PressureUnitType UnitType { get; set; }

        [ItemInfo(47)] public override decimal UnsqrFactor { get; set; }

        #endregion

        //public override ItemGroup SetValues(DeviceType deviceType, IEnumerable<ItemValue> itemValues)
        //{
        //    var items = itemValues.ToList();

        //    var site = deviceType.GetGroupValues<SiteInformationItems>(items);
            
        //    if (site.PressureFactor == CorrectionFactorType.Fixed) 
        //        return default;

        //    return base.SetValues(deviceType, items);
        //}
    }
}