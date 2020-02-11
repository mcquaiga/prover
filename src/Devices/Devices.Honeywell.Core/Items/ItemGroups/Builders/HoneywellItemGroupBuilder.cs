using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups.Builders;

namespace Devices.Honeywell.Core.Items.ItemGroups.Builders
{
    public class HoneywellItemGroupBuilder<TGroup> : ItemGroupBuilderBase<TGroup>
        where TGroup : IItemGroup
    {
        public HoneywellItemGroupBuilder(HoneywellDeviceType deviceType) : base(deviceType)
        {
        }
    }
}