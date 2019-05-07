namespace Devices.Core.Interfaces.Items
{
    public interface ITemperatureItems : IItemsGroup
    {
        decimal Base { get; }

        decimal Factor { get; }

        decimal GasTemperature { get; }

        TemperatureUnits Units { get; }
    }
}