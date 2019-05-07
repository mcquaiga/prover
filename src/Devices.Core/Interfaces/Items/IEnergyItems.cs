namespace Devices.Core.Interfaces.Items
{
    public interface IEnergyItems : IItemsGroup
    {
        decimal EnergyGasValue { get; }

        decimal EnergyReading { get; }

        string EnergyUnits { get; }
    }
}