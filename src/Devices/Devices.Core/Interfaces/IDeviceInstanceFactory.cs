using System.Collections.Generic;
using Devices.Core.Items;

namespace Devices.Core.Interfaces
{
    public interface IDeviceInstanceFactory
    {
        DeviceInstance CreateInstance(IEnumerable<ItemValue> itemValues = null);
        DeviceType DeviceType { get; }
    }
}