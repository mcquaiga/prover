using Devices.Core.Items.Attributes;
using Devices.Core.Items.ItemGroups;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    internal class SuperFactorItems : HoneywellItemGroup, ISuperFactorItems
    {
        [ItemInfo(55)]
        public decimal Co2 { get; protected set; }

        [ItemInfo(54)]
        public decimal N2 { get; protected set; }

        [ItemInfo(53)]
        public decimal SpecGr { get; protected set; }
    }
}