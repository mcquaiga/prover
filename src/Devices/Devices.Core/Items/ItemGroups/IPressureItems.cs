using System.Collections.Generic;
using Devices.Core.Interfaces.Items;

namespace Devices.Core.Items.ItemGroups
{
    public interface IPressureItems : IItemGroup
    {
        decimal AtmosphericPressure { get; set; }

        decimal Base { get; set; }

        decimal Factor { get; set; }

        decimal GasPressure { get; set; }

        int Range { get; set; }

        PressureTransducerType TransducerType { get; set; }

        PressureUnitType UnitType { get; set; }

        decimal UnsqrFactor { get; set; }
    }

    public abstract class PressureItems : IPressureItems
    {
        public decimal AtmosphericPressure { get; set; }
        public decimal Base { get; set; }
        public decimal Factor { get; set; }
        public decimal GasPressure { get; set; }
        public int Range { get; set; }
        public PressureTransducerType TransducerType { get; set; }
        public PressureUnitType UnitType { get; set; }
        public decimal UnsqrFactor { get; set; }
    }
}