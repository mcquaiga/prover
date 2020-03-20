using Devices.Core.Items.ItemGroups;
using Prover.Shared;

namespace Devices.Core.Interfaces
{
    public static class DeviceInstanceEx
    {
        public static CompositionType Composition(this DeviceInstance device) =>
            device.ItemGroup<SiteInformationItems>().CompositionType;

        public static bool HasLivePressure(this DeviceInstance device)
        {
            return device.Composition() == CompositionType.P || device.Composition() == CompositionType.PTZ;
        }

        public static bool HasLiveTemperature(this DeviceInstance device)
        {
            return device.Composition() == CompositionType.T || device.Composition() == CompositionType.PTZ;
        }
        
        public static bool HasLiveSuper(this DeviceInstance device)
        {
            return device.Composition() == CompositionType.PTZ;
        }
    }
}