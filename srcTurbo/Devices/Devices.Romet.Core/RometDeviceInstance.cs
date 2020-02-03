using System.Collections.Generic;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Honeywell.Core;

namespace Devices.Romet.Core
{

    public class RometDeviceInstanceFactory : IDeviceInstanceFactory
    {
        private readonly RometDeviceType _deviceType;

        public RometDeviceInstanceFactory(RometDeviceType deviceType)
        {
            _deviceType = deviceType;
        }

        public DeviceInstance CreateInstance(IEnumerable<ItemValue> itemValues = null)
        {
            var instance = new RometDeviceInstance(_deviceType);
            instance.SetItemGroups(itemValues);
            return instance;
        }
    }

    public class RometDeviceInstance : HoneywellDeviceInstance
    {
        public RometDeviceInstance(RometDeviceType deviceType) : base(deviceType)
        {
        }
    }
}