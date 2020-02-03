using Devices.Core;
using Devices.Core.Items.Attributes;
using Devices.Core.Items.ItemGroups;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    internal class TemperatureItems : HoneywellItemGroup, ITemperatureItems
    {
        [ItemInfo(34)]
        public decimal Base { get; protected set; }

        [ItemInfo(45)]
        public decimal Factor { get; protected set; }

        [ItemInfo(26)]
        public decimal GasTemperature { get; protected set; }

        [ItemInfo(89)]
        public TemperatureUnitType Units { get; protected set; }
    }
}