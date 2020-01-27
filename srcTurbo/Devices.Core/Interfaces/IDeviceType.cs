using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devices.Core.Interfaces
{
    public interface IDeviceType
    {
        bool? CanUseIrDaPort { get; }

        bool IsHidden { get; }

        ICollection<ItemMetadata> Items { get; }

        int? MaxBaudRate { get; }

        string Name { get; }

        IEnumerable<ItemValue> Convert<T>(IDictionary<T, string> values) where T : struct;

        IDeviceInstance CreateInstance(IEnumerable<ItemValue> itemValues);

        IEnumerable<ItemMetadata> GetItemNumbersByGroup<T>() where T : IItemsGroup;

        //T GetItemValuesByGroup<T>(IEnumerable<ItemValue> values) where T : IItemsGroup;
    }
}