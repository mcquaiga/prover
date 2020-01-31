using System.Collections.Generic;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;

namespace Devices.Core.Interfaces
{
    public interface IDeviceType
    {
        bool? CanUseIrDaPort { get; }

        bool IsHidden { get; }

        ICollection<ItemMetadata> Items { get; }

        int? MaxBaudRate { get; }

        string Name { get; }

        IDeviceInstanceFactory InstanceFactory { get; }


        IEnumerable<ItemMetadata> GetItemMetadataByGroup<T>() where T : IItemsGroup;
    }
}