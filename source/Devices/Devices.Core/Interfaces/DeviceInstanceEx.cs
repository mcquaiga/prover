using System;
using Devices.Core.Items.ItemGroups;
using Prover.Shared;

namespace Devices.Core.Interfaces
{
    public static class DeviceInstanceEx
    {
        public static CompositionType Composition(this DeviceInstance device) =>
            device.ItemGroup<SiteInformationItems>().CompositionType;

        public static string CompositionShort(this DeviceInstance device) => Composition(device).ToString();
        public static string CompositionDescription(this DeviceInstance device)
        {
            switch (Composition(device))
            {
                case CompositionType.T:
                    return "Temperature";
                case CompositionType.P:
                    return "Pressure";
                case CompositionType.PTZ:
                    return "Pressure & Temperature";
                case CompositionType.Fixed:
                    return "Fixed";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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

        public static string CompanyNumber(this DeviceInstance device)
        {
            return device.Items.SiteInfo.SiteId2;
        }
    }
}