using Devices.Core.Items.ItemGroups;

namespace Devices.Honeywell.Core
{
    public class PressureDeviceInstanceDecorator : DeviceInstanceDecorator
    {
        public PressureDeviceInstanceDecorator(HoneywellDeviceInstance deviceInstance) : base(deviceInstance)
        {
        }

        public IPressureItems PressureItems2 { get; set; }
    }
}