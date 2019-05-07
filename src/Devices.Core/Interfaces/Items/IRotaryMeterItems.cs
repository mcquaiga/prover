namespace Devices.Core.Interfaces.Items
{
    public interface IRotaryMeterItems : IItemsGroup
    {
        decimal MeterDisplacement { get; }

        string MeterModel { get; }

        int MeterModelId { get; }
    }
}