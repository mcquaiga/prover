using Devices.Core.Interfaces.Items;

namespace Devices.Core.Items.DriveTypes
{
    public interface IRotaryMeterItems : IItemGroup
    {
        MeterIndexItemDescription MeterType { get; set; }

        decimal MeterDisplacement { get; set; }
    }
}