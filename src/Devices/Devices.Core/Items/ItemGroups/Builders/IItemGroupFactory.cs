using System.Collections.Generic;

namespace Devices.Core.Items.ItemGroups.Builders
{
    public interface ItemGroupFactory
    {
        TGroup Create<TGroup>(IEnumerable<ItemValue> values) where TGroup : ItemGroup;
    }
}