using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Honeywell.Core.Devices;

namespace Devices.Honeywell.Core
{
    public class HoneywellDeviceInstanceFactory : IDeviceInstanceFactory
    {
        public readonly HoneywellDeviceType DeviceType;
        private HoneywellDeviceBuilder _deviceBuilder;

        protected HoneywellDeviceInstanceFactory()
        {

        }

        public HoneywellDeviceInstanceFactory(HoneywellDeviceType deviceType)
        {
            DeviceType = deviceType;
        }

        #region Public Methods

        public virtual DeviceInstance CreateInstance(IEnumerable<ItemValue> itemValues = null)
        {
            return CreateWithBuilder(itemValues);
        }

        public static IDeviceInstanceFactory Find(HoneywellDeviceType honeywellDeviceType)
        {
            return new HoneywellDeviceInstanceFactory(honeywellDeviceType);
        }

        #endregion

        #region Private

        private DeviceInstance CreateBasic(IEnumerable<ItemValue> itemValues = null)
        {
            var instance = new HoneywellDeviceInstance(DeviceType);
            instance.SetItemValues(itemValues);

            return instance;
        }

        private DeviceInstance CreateWithBuilder(IEnumerable<ItemValue> itemValues = null)
        {
            var values = itemValues as ItemValue[] ?? itemValues.ToArray();

            _deviceBuilder = new HoneywellDeviceBuilder(DeviceType, values);

            _deviceBuilder
                .BuildPtz()
                .BuildDriveType();

            return _deviceBuilder.GetDeviceInstance();
        }

        #endregion
    }
}