using Devices.Core;
using Devices.Core.Items.Attributes;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    public class EnergyItems : HoneywellItemGroup, IEnergyItems
    {
        [ItemInfo(142)]
        public decimal EnergyGasValue { get; protected set; }

        [ItemInfo(140)]
        public decimal EnergyReading { get; protected set; }

        [ItemInfo(141)]
        public EnergyUnitType EnergyUnitType { get; protected set; }
    }
}