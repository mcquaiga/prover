using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Prover.Domain.EvcVerifications;
using Prover.Shared;
using ReactiveUI;

namespace Prover.Application.ViewModels
{
    public class DeviceInfoViewModel : ReactiveObject
    {
        public DeviceInfoViewModel(DeviceInstance device)
        {
            Device = device;
           
        }

        public DeviceInstance Device { get; }
        public SiteInformationItems SiteInfo => Device.Items.SiteInfo;
        public PressureItems Pressure => Device.Items.Pressure;
        public TemperatureItems Temperature => Device.Items.Temperature;
        public SuperFactorItems SuperFactor => Device.Items.SuperFactor;
        public PulseOutputItems PulseOutput => Device.Items.PulseOutput;
        public VolumeItems Volume => Device.Items.Volume;

        public string CompanyNumber => SiteInfo.SiteId2;
    }

    public class SiteInformationViewModel : DeviceInfoViewModel
    {
        public EvcVerificationViewModel Test { get; }

        public SiteInformationViewModel(DeviceInstance device, EvcVerificationViewModel verificationViewModel) : base(device)
        {
            Test = verificationViewModel;
        }

        //public PulseOutputItems.ChannelItems PulseOutputChannelA => PulseOutput.GetChannel(PulseOutputChannel.Channel_A);
        //public PulseOutputItems.ChannelItems PulseOutputChannelB => PulseOutput.GetChannel(PulseOutputChannel.Channel_A);
    }
}