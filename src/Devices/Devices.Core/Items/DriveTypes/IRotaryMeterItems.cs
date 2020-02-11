using Devices.Core.Interfaces;
using Devices.Core.Items.Descriptions;

namespace Devices.Core.Items.DriveTypes
{
    public interface IRotaryMeterItems : IItemGroup
    {
        RotaryMeterTypeDescription MeterType { get; set; }

        decimal MeterDisplacement { get; set; }
    }
}