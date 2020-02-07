using System.Collections.Generic;
using Devices.Core.Interfaces.Items;

namespace Devices.Core.Items.ItemGroups.Builders
{
    public interface IItemGroupFactory
    {
        TGroup Create<TGroup>(IEnumerable<ItemValue> values) where TGroup : IItemGroup;
    }
}