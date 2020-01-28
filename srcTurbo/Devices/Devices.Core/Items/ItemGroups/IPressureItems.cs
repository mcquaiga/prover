using Devices.Core.Interfaces.Items;

namespace Devices.Core.Items.ItemGroups
{
    public interface IPressureItems : IItemsGroup
    {
        decimal AtmosphericPressure { get; }

        decimal Base { get; }

        decimal Factor { get; }

        decimal GasPressure { get; }

        int Range { get; }

        PressureTransducerType TransducerType { get; }

        PressureUnitType UnitType { get; }

        decimal UnsqrFactor { get; }
    }
}