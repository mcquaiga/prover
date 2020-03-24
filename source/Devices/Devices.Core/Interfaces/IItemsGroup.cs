using System.Collections.Generic;
using Devices.Core.Items;

namespace Devices.Core.Interfaces
{
    public interface IHaveItemInformation
    {
        ICollection<ItemMetadata> RelatedItems();
    }
}