using Devices.Core.Items.Attributes;
using Devices.Core.Items.ItemGroups;
using Prover.Shared;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    public class TemperatureItemsHoneywell : TemperatureItems
    {
        [ItemInfo(34)]
        public override decimal Base { get; set; }

        [ItemInfo(45)]
        public override decimal Factor { get; set; }

        [ItemInfo(26)]
        public override decimal GasTemperature { get; set; }

        [ItemInfo(89)]
        public override TemperatureUnitType Units { get; set; }
    }
}