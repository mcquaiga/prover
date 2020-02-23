using System;
using System.Collections.Generic;
using System.Linq;
using Application.ViewModels.Corrections;
using Application.ViewModels.Volume;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Domain.EvcVerifications.Verifications.Volume.InputTypes;
using ReactiveUI.Fody.Helpers;
using Shared;

namespace Application.ViewModels
{
    public class EvcVerificationViewModel : BaseViewModel
    {
        private EvcVerificationViewModel()
        {
        }

        [Reactive] public DeviceInstance Device { get; set; }
        [Reactive] public DeviceType DeviceType { get; set; }

        [Reactive] public CompositionType CompositionType { get; set; }

        [Reactive] public IVolumeInputType DriveType { get; set; }

        [Reactive] public DateTime TestDateTime { get; set; } = DateTime.Now;

        [Reactive]public DateTime? ExportedDateTime { get; set; }

        [Reactive] public DateTime? ArchivedDateTime { get; set; }

        public ICollection<VerificationTestPointViewModel> Tests { get; set; } =
            new List<VerificationTestPointViewModel>();

        [Reactive]
        public ICollection<VerificationViewModel> OtherTests { get; set; } = new List<VerificationViewModel>();

        public SiteInformationItems SiteInfo { get; protected set; }

        //public PressureItems Pressure { get; protected set; }

        //public TemperatureItems Temperature { get; protected set; }

        //public SuperFactorItems SuperFactor { get; protected set; }

        //public PulseOutputItems PulseOutput { get; protected set; }

        //public VolumeItems Volume { get; protected set; }


        public VolumeViewModel VolumeTest => Tests.FirstOrDefault(t => t.Volume != null)?.Volume;

        public void Initialize()
        {
            SiteInfo = Device.ItemGroup<SiteInformationItems>();
            //Pressure = Device.ItemGroup<PressureItems>();
            //Temperature = Device.ItemGroup<TemperatureItems>();
            //SuperFactor = Device.ItemGroup<SuperFactorItems>();
            //Volume = Device.ItemGroup<VolumeItems>();
            //PulseOutput = Device.ItemGroup<PulseOutputItems>();
        }
    }
}