using Devices.Core;
using Devices.Core.Items.Attributes;
using Devices.Core.Items.DriveTypes;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    public class EnergyItemsHoneywell : EnergyItems
    {
        [ItemInfo(142)]
        public override decimal EnergyGasValue { get; set; }

        [ItemInfo(140)]
        public override decimal EnergyReading { get; set; }

        [ItemInfo(141)]
        public override EnergyUnitType EnergyUnitType { get; set; }
    }
}