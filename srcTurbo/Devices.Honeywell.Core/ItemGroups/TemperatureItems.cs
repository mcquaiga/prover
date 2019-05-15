using Devices.Core;
using Devices.Core.Interfaces.Items;
using Devices.Honeywell.Core.Attributes;

namespace Devices.Honeywell.Core.ItemGroups
{
    internal class TemperatureItems : ItemGroupBase, ITemperatureItems
    {
        [ItemInfo(34)]
        public decimal Base { get; protected set; }

        [ItemInfo(45)]
        public decimal Factor { get; protected set; }

        [ItemInfo(26)]
        public decimal GasTemperature { get; protected set; }

        [ItemInfo(89)]
        public TemperatureUnits Units { get; protected set; }
    }
}