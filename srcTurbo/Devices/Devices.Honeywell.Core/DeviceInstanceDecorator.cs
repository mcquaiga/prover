using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;

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

    public class PressureDeviceInstanceDecorator : DeviceInstanceDecorator
    {
        public PressureDeviceInstanceDecorator(HoneywellDeviceInstance deviceInstance) : base(deviceInstance)
        {
        }

        public IPressureItems PressureItems2 { get; set; }
    }

    public class MechanicalDeviceInstanceDecorator : DeviceInstanceDecorator
    {
        public MechanicalDeviceInstanceDecorator(HoneywellDeviceInstance deviceInstance) : base(deviceInstance)
        {
        }

        #region Public Properties

        public IEnergyItems EnergyItems { get; set; }

        #endregion
    }
}