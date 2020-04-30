using System.Collections.Generic;
using Devices.Core.Interfaces;

namespace Devices.Core.Items.ItemGroups.Builders
{
    public interface ItemGroupBuilder<out TGroup> 
        where TGroup : ItemGroup
    {
        //TGroup Build<T>(DeviceType device, IEnumerable<ItemValue> values) where T : ItemGroup;
        TGroup Build(DeviceType device, IEnumerable<ItemValue> values);
        TGroup Build<T>(DeviceType device, IEnumerable<ItemValue> values) where T : ItemGroup;
    }
}