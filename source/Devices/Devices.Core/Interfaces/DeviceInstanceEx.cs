using System;
using Devices.Core.Items.ItemGroups;
using Prover.Shared;

namespace Devices.Core.Interfaces
{
    public static class DeviceInstanceEx
    {
        public static string CompanyNumber(this DeviceInstance device) => device.Items.SiteInfo.SiteId2;

        public static CompositionType Composition(this DeviceInstance device) =>
            device.ItemGroup<SiteInformationItems>().CompositionType;

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

        public static string CompositionShort(this DeviceInstance device) => Composition(device).ToString();

        public static bool HasLivePressure(this DeviceInstance device) =>
            device.Composition() == CompositionType.P || device.Composition() == CompositionType.PTZ;

        public static bool HasLiveSuper(this DeviceInstance device) => device.Composition() == CompositionType.PTZ;

        public static bool HasLiveTemperature(this DeviceInstance device) =>
            device.Composition() == CompositionType.T || device.Composition() == CompositionType.PTZ;

        public static DeviceItems Items(this DeviceInstance device) => new DeviceItems(device);
        public static PressureItems Pressure(this DeviceInstance device) => device.ItemGroup<PressureItems>();
        public static PulseOutputItems PulseOutput(this DeviceInstance device) => device.ItemGroup<PulseOutputItems>();

        public static SiteInformationItems SiteInfo(this DeviceInstance device) =>
            device.ItemGroup<SiteInformationItems>();

        public static SuperFactorItems SuperFactor(this DeviceInstance device) => device.ItemGroup<SuperFactorItems>();
        public static TemperatureItems Temperature(this DeviceInstance device) => device.ItemGroup<TemperatureItems>();
        public static VolumeItems Volume(this DeviceInstance device) => device.ItemGroup<VolumeItems>();
    }

    public class DeviceItems
    {
        private readonly DeviceInstance _device;
        public DeviceItems(DeviceInstance device) => _device = device;
        public SiteInformationItems SiteInfo => _device.ItemGroup<SiteInformationItems>();
        public PressureItems Pressure => _device.ItemGroup<PressureItems>();
        public TemperatureItems Temperature => _device.ItemGroup<TemperatureItems>();
        public SuperFactorItems SuperFactor => _device.ItemGroup<SuperFactorItems>();
        public PulseOutputItems PulseOutput => _device.ItemGroup<PulseOutputItems>();
        public VolumeItems Volume => _device.ItemGroup<VolumeItems>();
    }
}