using Devices.Core.Interfaces.Items;

namespace Devices.Core.Items.ItemGroups
{
    public interface ITemperatureItems : IItemsGroup
    {
        decimal Base { get; }

        decimal Factor { get; }

        decimal GasTemperature { get; }

        TemperatureUnitType Units { get; }
    }
}