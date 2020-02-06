using System.Collections.Generic;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;

namespace Devices.Core.Items.ItemGroups.Builders
{
    public interface IBuildItemsFor<out T>
        where T : IItemGroup
    {
        T Build(DeviceType device, IEnumerable<ItemValue> values);
    }
}