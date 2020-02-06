using Devices.Core;
using Devices.Core.Items.Attributes;
using Devices.Core.Items.ItemGroups;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    public class PressureItems : HoneywellItemGroup, IPressureItems
    {
        #region Public Properties

        [ItemInfo(14)] public decimal AtmosphericPressure { get; set; }

        [ItemInfo(13)] public decimal Base { get; set; }

        [ItemInfo(44)] public decimal Factor { get; set; }

        [ItemInfo(8)] public decimal GasPressure { get; set; }

        [ItemInfo(137)] public int Range { get; set; }

        [ItemInfo(112)] public PressureTransducerType TransducerType { get; set; }

        [ItemInfo(87)] public PressureUnitType UnitType { get; set; }

        [ItemInfo(47)] public decimal UnsqrFactor { get; set; }

        #endregion
    }
}