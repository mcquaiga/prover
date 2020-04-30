using System.Collections.Generic;
using Devices.Core.Items;

namespace Devices.Honeywell.Core.Devices
{
    public class HoneywellDeviceBuilder : DeviceBuilder
    {
        public HoneywellDeviceBuilder(HoneywellDeviceType deviceType, IEnumerable<ItemValue> itemValues) 
            : base(deviceType)
        {
            SetItemValues(itemValues);
        }

        protected sealed override DeviceBuilder SetItemValues(IEnumerable<ItemValue> values)
        {
            DeviceInstance.SetItemValues(values);
            return this;
        }
    }
}