using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Prover.Domain.EvcVerifications;
using ReactiveUI;

namespace Prover.Application.ViewModels
{
    public class DeviceInfoViewModel : ReactiveObject
    {
        public DeviceInfoViewModel(DeviceInstance device)
        {
            Device = device;
            SiteInfo = device.ItemGroup<SiteInformationItems>();
            Pressure = device.ItemGroup<PressureItems>();
            Temperature = device.ItemGroup<TemperatureItems>();
            SuperFactor = device.ItemGroup<SuperFactorItems>();
            Volume = device.ItemGroup<VolumeItems>();
            PulseOutput = device.ItemGroup<PulseOutputItems>();
        }

        public DeviceInstance Device { get; }
        public SiteInformationItems SiteInfo { get; protected set; }
        public PressureItems Pressure { get; protected set; }
        public TemperatureItems Temperature { get; protected set; }
        public SuperFactorItems SuperFactor { get; protected set; }
        public PulseOutputItems PulseOutput { get; protected set; }
        public VolumeItems Volume { get; protected set; }

        public string CompanyNumber => SiteInfo.SiteId2;
    }

    public class SiteInformationViewModel : DeviceInfoViewModel
    {
        public EvcVerificationViewModel Test { get; }

        public SiteInformationViewModel(DeviceInstance device, EvcVerificationViewModel verificationViewModel) : base(device)
        {
            Test = verificationViewModel;
        }
    }
}