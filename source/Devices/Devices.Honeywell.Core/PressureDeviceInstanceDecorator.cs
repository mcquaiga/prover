using Devices.Core.Items.ItemGroups;

namespace Devices.Honeywell.Core
{
    public class PressureDeviceInstanceDecorator : DeviceInstanceDecorator
    {
        public PressureDeviceInstanceDecorator(HoneywellDeviceInstance deviceInstance) : base(deviceInstance)
        {
        }

        public PressureItems PressureItems2 { get; set; }
    }
}