using System.Collections.Generic;
using Devices.Core.Interfaces;

namespace Devices.Core.Items.ItemGroups.Builders
{
    public interface IBuildItemsGroup
    {
        void Build(DeviceType device, IEnumerable<ItemValue> values);
    }

    public interface IBuildItemsFor<out T>
        where T : ItemGroup
    {
        T Build(DeviceType device, IEnumerable<ItemValue> values);
    }
}