using Devices.Core.Interfaces.Items;

namespace Devices.Core.Items.DriveTypes
{
    public interface IRotaryMeterItems : IItemsGroup
    {
        decimal MeterDisplacement { get; }

        string MeterModel { get; }

        int MeterModelId { get; }
    }
}