using Devices.Core.Interfaces.Items;

namespace Devices.Core.Items.DriveTypes
{
    public interface IEnergyItems : IItemsGroup
    {
        decimal EnergyGasValue { get; }

        decimal EnergyReading { get; }

        EnergyUnitType EnergyUnitType { get; }
    }
}