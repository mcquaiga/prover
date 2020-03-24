using Devices.Core.Items.Attributes;
using Devices.Core.Items.Descriptions;
using Devices.Core.Items.DriveTypes;

namespace Devices.Romet.Core.Items.ItemGroups
{
    internal class RotaryMeterItemsRomet : RotaryMeterItems
    {
        [ItemInfo(768)]
        public override RotaryMeterTypeDescription MeterType { get; set; }


        public override decimal MeterDisplacement { get; set; }
    }
}
