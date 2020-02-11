using Devices.Core.Interfaces;

namespace Devices.Core.Items.ItemGroups
{
    public interface IPressureCorrectionItems : IHaveFactor
    {
        decimal AtmosphericPressure { get; set; }
        decimal GasPressure { get; set; }
        decimal UnsqrFactor { get; set; }
    }

    public interface IPressureItems : IItemGroup, IPressureCorrectionItems
    {
        decimal Base { get; set; }

        int Range { get; set; }

        PressureTransducerType TransducerType { get; set; }

        PressureUnitType UnitType { get; set; }
    }

    public abstract class PressureItemsBase : ItemGroup, IPressureItems
    {
        public abstract decimal AtmosphericPressure { get; set; }
        public abstract decimal Base { get; set; }
        public abstract decimal Factor { get; set; }
        public abstract decimal GasPressure { get; set; }
        public abstract int Range { get; set; }
        public abstract PressureTransducerType TransducerType { get; set; }
        public abstract PressureUnitType UnitType { get; set; }
        public abstract decimal UnsqrFactor { get; set; }
    }
}