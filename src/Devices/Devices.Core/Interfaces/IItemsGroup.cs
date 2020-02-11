using System.Collections.Generic;
using System.Reflection;
using Devices.Core.Items;

namespace Devices.Core.Interfaces
{
    public interface IHaveItemInformation
    {
        ICollection<ItemMetadata> RelatedItems();
    }

    public interface IItemGroup
    {
        void SetValue(PropertyInfo property, ItemValue value);
    }
}