namespace Devices.Honeywell.Core
{
    public abstract class DeviceInstanceDecorator : HoneywellDeviceInstance
    {
        protected readonly HoneywellDeviceInstance DeviceInstance;

        protected DeviceInstanceDecorator(HoneywellDeviceInstance deviceInstance) : base(deviceInstance.DeviceType)
        {
            DeviceInstance = deviceInstance;
        }
    }
}