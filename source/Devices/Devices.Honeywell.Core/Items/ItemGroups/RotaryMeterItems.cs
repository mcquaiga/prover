using Devices.Core.Items.Attributes;
using Devices.Core.Items.Descriptions;
using Devices.Core.Items.DriveTypes;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    public class RotaryMeterItemsHoneywell : RotaryMeterItems
    {
        [ItemInfo(432)]
        public override RotaryMeterTypeDescription MeterType { get; set; }

        [ItemInfo(439)]
        public override decimal MeterDisplacement { get; set; }
    }
}
