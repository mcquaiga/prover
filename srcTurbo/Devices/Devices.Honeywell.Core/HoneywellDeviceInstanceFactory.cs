using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Honeywell.Core.Devices;
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
        private HoneywellDeviceBuilder _deviceBuilder;

        public HoneywellDeviceInstanceFactory(HoneywellDeviceType deviceType)
        {
            DeviceType = deviceType;
            
        }

        public virtual DeviceInstance CreateInstance(IEnumerable<ItemValue> itemValues = null)
        {
            var values = itemValues as ItemValue[] ?? itemValues.ToArray();
            
            _deviceBuilder = new HoneywellDeviceBuilder(DeviceType, values);
            
            _deviceBuilder
                .BuildPtz()
                .BuildAttributes()
                .BuildDriveType();

            return _deviceBuilder.GetDeviceInstance();
        }

        public static IDeviceInstanceFactory Find(HoneywellDeviceType honeywellDeviceType)
        {
            return new HoneywellDeviceInstanceFactory(honeywellDeviceType);

            if (_factories.TryGetValue(honeywellDeviceType.Name, out var factory))
            {
                return factory.Invoke(honeywellDeviceType);
            }
        }
    }

    public class MiniAtDeviceInstanceFactory : HoneywellDeviceInstanceFactory
    {
        public MiniAtDeviceInstanceFactory(HoneywellDeviceType deviceType) : base(deviceType)
        {
        }

        public override DeviceInstance CreateInstance(IEnumerable<ItemValue> itemValues = null)
        {
            return base.CreateInstance(itemValues);
        }
    }
}