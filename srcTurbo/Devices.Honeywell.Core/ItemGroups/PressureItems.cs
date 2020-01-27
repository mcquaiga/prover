using Devices.Core;
using Devices.Core.Interfaces.Items;
using Devices.Honeywell.Core.Attributes;

namespace Devices.Honeywell.Core.ItemGroups
{
    internal class PressureItems : ItemGroupBase, IPressureItems
    {
        [ItemInfo(14)]
        public decimal AtmosphericPressure { get; protected set; }

        [ItemInfo(13)]
        public decimal Base { get; protected set; }

        [ItemInfo(44)]
        public decimal Factor { get; protected set; }

        [ItemInfo(8)]
        public decimal GasPressure { get; protected set; }

        [ItemInfo(137)]
        public int Range { get; protected set; }

        [ItemInfo(112)]
        public PressureTransducerType TransducerType { get; protected set; }

        [ItemInfo(87)]
        public PressureUnitType UnitType { get; protected set; }

        [ItemInfo(47)]
        public decimal UnsqrFactor { get; protected set; }
    }
}