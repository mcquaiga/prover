using Devices.Core;
using Devices.Core.Items.Attributes;
using Devices.Core.Items.ItemGroups;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    public class TemperatureItems : HoneywellItemGroup, ITemperatureItems
    {
        [ItemInfo(34)]
        public decimal Base { get; set; }

        [ItemInfo(45)]
        public decimal Factor { get; set; }

        [ItemInfo(26)]
        public decimal GasTemperature { get; set; }

        [ItemInfo(89)]
        public TemperatureUnitType Units { get; set; }
    }
}