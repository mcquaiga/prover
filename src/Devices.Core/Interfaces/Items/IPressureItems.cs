namespace Devices.Core.Interfaces.Items
{
    public interface IPressureItems : IItemsGroup
    {
        decimal AtmosphericPressure { get; }

        decimal Base { get; }

        decimal Factor { get; }

        decimal GasPressure { get; }

        int Range { get; }

        PressureTransducerType TransducerType { get; }

        PressureUnits Units { get; }

        decimal UnsqrFactor { get; }
    }
}