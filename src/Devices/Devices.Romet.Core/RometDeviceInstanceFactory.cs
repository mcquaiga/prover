using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Honeywell.Core.Devices;

namespace Devices.Romet.Core
{
    public class RometDeviceInstanceFactory : IDeviceInstanceFactory
    {
        private readonly RometDeviceType _deviceType;
        private HoneywellDeviceBuilder _deviceBuilder;

        protected RometDeviceInstanceFactory()
        {

        }

        public RometDeviceInstanceFactory(RometDeviceType deviceType)
        {
            _deviceType = deviceType;
        }

        #region Public Methods

        public DeviceInstance CreateInstance(IEnumerable<ItemValue> itemValues = null)
        {
            var values = itemValues as ItemValue[] ?? itemValues.ToArray();

            _deviceBuilder = new HoneywellDeviceBuilder(_deviceType, values);

            _deviceBuilder
                .BuildPtz()
                .BuildDriveType();

            return _deviceBuilder.GetDeviceInstance();
        }

        #endregion
    }
}