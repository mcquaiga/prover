using Devices.Core.Items;
using System.Collections.Generic;

namespace Devices.Core.Interfaces.Items
{
    public interface IHaveItemInformation
    {
        ICollection<ItemMetadata> RelatedItems();
    }

    public interface IItemsGroup
    {
        void SetValues(IEnumerable<ItemValue> values);
    }
}