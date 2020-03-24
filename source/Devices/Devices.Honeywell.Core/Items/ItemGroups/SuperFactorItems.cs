using Devices.Core.Items.Attributes;
using Devices.Core.Items.ItemGroups;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    public class SuperFactorItemsHoneywell : SuperFactorItems
    {
        [ItemInfo(55)]
        public override decimal Co2 { get; set; }

        [ItemInfo(54)]
        public override decimal N2 { get; set; }

        [ItemInfo(53)]
        public override decimal SpecGr { get; set; }

        [ItemInfo(47)]
        public override decimal Factor { get; set; }
    }
}