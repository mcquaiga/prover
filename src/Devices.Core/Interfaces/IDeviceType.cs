using Devices.Core.Items;
using System.Collections.Generic;

namespace Devices.Core.Interfaces
{
    public interface IDeviceType
    {
        #region Properties

        bool? CanUseIrDaPort { get; }

        ICollection<ItemMetadata> Definitions { get; }

        bool IsHidden { get; }

        string ItemFilePath { get; set; }

        int? MaxBaudRate { get; }

        string Name { get; }

        IDevice CreateInstance();

        IDevice CreateInstance(Dictionary<string, string> itemValues);

        #endregion
    }
}