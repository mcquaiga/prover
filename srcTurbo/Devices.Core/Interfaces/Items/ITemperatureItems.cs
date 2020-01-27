namespace Devices.Core.Interfaces.Items
{
    public interface ITemperatureItems : IItemsGroup
    {
        decimal Base { get; }

        decimal Factor { get; }

        decimal GasTemperature { get; }

        TemperatureUnitType Units { get; }
    }
}