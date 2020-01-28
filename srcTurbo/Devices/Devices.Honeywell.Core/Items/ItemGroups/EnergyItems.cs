using Devices.Core;
using Devices.Core.Items.DriveTypes;
using Devices.Honeywell.Core.Attributes;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    internal class EnergyItems : ItemGroupBase, IEnergyItems
    {
        [ItemInfo(142)]
        public decimal EnergyGasValue { get; protected set; }

        [ItemInfo(140)]
        public decimal EnergyReading { get; protected set; }

        [ItemInfo(141)]
        public EnergyUnitType EnergyUnitType { get; protected set; }
    }
}