using System.Collections.Generic;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Honeywell.Core.Items.ItemGroups;
using NLog.LayoutRenderers.Wrappers;

namespace Devices.Honeywell.Core
{
    public class HoneywellDeviceInstanceFactory 
        : IDeviceInstanceFactory
    {
        public readonly IHoneywellDeviceType DeviceType;

        public HoneywellDeviceInstanceFactory(IHoneywellDeviceType deviceType)
        {
            DeviceType = deviceType;
        }

        public IDeviceInstance CreateInstance(IEnumerable<ItemValue> itemValues = null) 
           
        {
            if (itemValues == null)
                itemValues = new List<ItemValue>();

            var instance = new HoneywellDeviceInstance { DeviceType = this.DeviceType };
            instance.SetItemValueGroups(itemValues);
            return instance;
        }
    }
}