using Devices.Core.Items.Descriptions;
using Devices.Core.Items.ItemGroups;

namespace Devices.Core.Items.DriveTypes
{
    public class RotaryMeterItems : ItemGroup
    {
        public virtual RotaryMeterTypeDescription MeterType { get; set; }
        public virtual decimal MeterDisplacement { get; set; }
    }
}