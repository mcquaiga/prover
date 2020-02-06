using System.Collections.Generic;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;

namespace Devices.Core.Items.ItemGroups.Builders
{
    public interface IItemGroupBuilder<out TGroup> 
        where TGroup : IItemGroup
    {
        //TGroup Build<T>(DeviceType device, IEnumerable<ItemValue> values) where T : IItemGroup;
        TGroup Build(DeviceType device, IEnumerable<ItemValue> values);
        TGroup Build<T>(DeviceType device, IEnumerable<ItemValue> values) where T : IItemGroup;
    }
}