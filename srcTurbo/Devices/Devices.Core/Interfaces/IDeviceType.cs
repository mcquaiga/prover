using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using System.Collections.Generic;

namespace Devices.Core.Interfaces
{
    public interface IDeviceType
    {
        bool? CanUseIrDaPort { get; }

        bool IsHidden { get; }

        ICollection<ItemMetadata> Items { get; }

        int? MaxBaudRate { get; }

        string Name { get; }

        IDeviceInstance CreateDeviceInstance(IEnumerable<ItemValue> itemValues);

        IDeviceInstance CreateDeviceInstance(IDictionary<int, string> itemValuesDictionary);

        IEnumerable<ItemMetadata> GetItemMetadataByGroup<T>() where T : IItemsGroup;
    }
}