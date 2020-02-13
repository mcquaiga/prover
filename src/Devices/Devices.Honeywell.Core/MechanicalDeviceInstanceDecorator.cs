using Devices.Core.Items.DriveTypes;

namespace Devices.Honeywell.Core
{
    public class MechanicalDeviceInstanceDecorator : DeviceInstanceDecorator
    {
        public MechanicalDeviceInstanceDecorator(HoneywellDeviceInstance deviceInstance) : base(deviceInstance)
        {
        }

        #region Public Properties

        public EnergyItems EnergyItems { get; set; }

        #endregion
    }
}