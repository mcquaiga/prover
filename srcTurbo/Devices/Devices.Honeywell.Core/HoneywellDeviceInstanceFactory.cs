using System;
using System.Collections.Generic;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Honeywell.Core.Items;
using Devices.Honeywell.Core.Items.ItemGroups;
using NLog.LayoutRenderers.Wrappers;

namespace Devices.Honeywell.Core
{
    public class HoneywellDeviceInstanceFactory : IDeviceInstanceFactory
    {
        private static readonly Dictionary<string, Func<HoneywellDeviceType,HoneywellDeviceInstanceFactory>> _factories = new Dictionary<string, Func<HoneywellDeviceType,HoneywellDeviceInstanceFactory>>()
        {
            { "Mini-AT", type => new MiniAtDeviceInstanceFactory(type) },

        };

        public readonly HoneywellDeviceType DeviceType;

        public HoneywellDeviceInstanceFactory(HoneywellDeviceType deviceType)
        {
            DeviceType = deviceType;
        }

        public DeviceInstance CreateInstance(IEnumerable<ItemValue> itemValues = null)
        {
            if (itemValues == null)
                itemValues = new List<ItemValue>();

            var instance = new HoneywellDeviceInstance(this.DeviceType);
            instance.SetItemGroups(itemValues);
            return instance;
        }

        public static IDeviceInstanceFactory Find(HoneywellDeviceType honeywellDeviceType)
        {
            if (_factories.TryGetValue(honeywellDeviceType.Name, out var factory))
            {
                return factory.Invoke(honeywellDeviceType);
            }
            return new HoneywellDeviceInstanceFactory(honeywellDeviceType);
        }
    }

    public class MiniAtDeviceInstanceFactory : HoneywellDeviceInstanceFactory
    {
        public MiniAtDeviceInstanceFactory(HoneywellDeviceType deviceType) : base(deviceType)
        {
        }
    }
}