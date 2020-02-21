using Devices.Core.Items.Attributes;
using Devices.Core.Items.ItemGroups;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    public class PulseOutputItemsHoneywell : PulseOutputItems
    {
        [ItemInfo(5)] public override int A_Count { get; set; }

        [ItemInfo(6)] public override int B_Count { get; set; }
        [ItemInfo(93)] public override string A_Units { get; set; }
        [ItemInfo(94)] public override string B_Units { get; set; }
        [ItemInfo(95)] public override string C_Units { get; set; }

        [ItemInfo(56)] public override decimal A_Scaling { get; set; }
        [ItemInfo(57)] public override decimal B_Scaling { get; set; }
        [ItemInfo(58)] public override decimal C_Scaling { get; set; }
    }
}