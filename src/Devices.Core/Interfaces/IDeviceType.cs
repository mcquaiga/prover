using Devices.Core.Items;
using System.Collections.Generic;

namespace Devices.Core.Interfaces
{
    public interface IDeviceType
    {
        bool? CanUseIrDaPort { get; }

        ICollection<ItemMetadata> Definitions { get; }

        bool IsHidden { get; }

        string ItemFilePath { get; set; }

        int? MaxBaudRate { get; }

        string Name { get; }

        IDevice CreateInstance(Dictionary<string, string> itemValues);

        IDevice CreateInstance(IEnumerable<ItemValue> itemValues);
    }
}