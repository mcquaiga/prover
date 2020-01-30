using System.Collections.Generic;
using Devices.Core.Items;

namespace Devices.Core.Interfaces
{
    public interface IDeviceInstanceFactory
    //<out TInstance>
    //where TInstance : IDeviceInstance
    {
        IDeviceInstance CreateInstance(IEnumerable<ItemValue> itemValues = null);
    }
}