using Devices.Core.Items;
using Devices.Core.Items.Attributes;
using Devices.Core.Items.Descriptions;
using Devices.Core.Items.DriveTypes;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    public class RotaryMeterItems : HoneywellItemGroup, IRotaryMeterItems
    {
        [ItemInfo(432)]
        public RotaryMeterTypeDescription MeterType { get; set; }

        [ItemInfo(439)]
        public decimal MeterDisplacement { get; set; }
    }
}
