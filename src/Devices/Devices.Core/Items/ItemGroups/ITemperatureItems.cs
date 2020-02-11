using Devices.Core.Interfaces;

namespace Devices.Core.Items.ItemGroups
{
    public interface ITemperatureItems : IHaveFactor, IItemGroup
    {
        decimal Base { get; }

        decimal GasTemperature { get; }

        TemperatureUnitType Units { get; }
    }
}