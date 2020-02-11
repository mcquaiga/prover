using System;
using System.Collections.Generic;
using System.Text;
using Devices.Core.Items.Attributes;
using Devices.Core.Items.Descriptions;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;

namespace Devices.Romet.Core.Items.ItemGroups
{
    internal class RotaryMeterItems : ItemGroup, IRotaryMeterItems
    {
        [ItemInfo(768)]
        public RotaryMeterTypeDescription MeterType { get; set; }


        public decimal MeterDisplacement { get; set; }
    }
}
