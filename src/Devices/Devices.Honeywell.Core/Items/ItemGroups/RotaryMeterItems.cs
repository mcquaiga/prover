using System;
using System.Collections.Generic;
using System.Text;
using Devices.Core.Items;
using Devices.Core.Items.Attributes;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    public class RotaryMeterItems : HoneywellItemGroup, IRotaryMeterItems
    {
        [ItemInfo(432)]
        public MeterIndexItemDescription MeterType { get; set; }

        [ItemInfo(439)]
        public decimal MeterDisplacement { get; set; }
    }
}
