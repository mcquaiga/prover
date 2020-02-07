using Devices.Core.Items;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devices.Core.Interfaces.Items
{
    public interface IHaveItemInformation
    {
        ICollection<ItemMetadata> RelatedItems();
    }

    public interface IItemGroup
    {
        //void SetValues(IEnumerable<ItemValue> values);
    }
}