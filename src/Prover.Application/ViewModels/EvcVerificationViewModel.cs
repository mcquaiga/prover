using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Prover.Application.Extensions;
using Prover.Application.ViewModels.Corrections;
using Prover.Application.ViewModels.Volume;
using Prover.Domain.EvcVerifications.Verifications.Volume.InputTypes;
using Prover.Shared;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels
{
    public class EvcVerificationViewModel : ViewModelWithIdBase, IDisposable
    {
        private EvcVerificationViewModel()
        {
        }

        [Reactive] public DeviceInstance Device { get; set; }

        [Reactive] public CompositionType CompositionType { get; set; }

        [Reactive] public IVolumeInputType DriveType { get; set; }

        [Reactive] public DateTime TestDateTime { get; set; } = DateTime.Now;

        [Reactive]public DateTime? ExportedDateTime { get; set; }

        [Reactive] public DateTime? ArchivedDateTime { get; set; }

        public ICollection<VerificationTestPointViewModel> Tests { get; set; } =
            new List<VerificationTestPointViewModel>();

        [Reactive]
        public ICollection<VerificationViewModel> OtherTests { get; set; } = new List<VerificationViewModel>();

        public DeviceInfoViewModel DeviceInfo { get; set; }

        public VolumeViewModelBase VolumeTest => Tests.FirstOrDefault(t => t.Volume != null)?.Volume;

        public void Initialize()
        {
            DeviceInfo = new DeviceInfoViewModel(Device, this);
        }

        protected override void Disposing()
        {
            Tests.ForEach(t => t.DisposeWith(Cleanup));
            OtherTests.ForEach(t => t.DisposeWith(Cleanup));
            Tests.Clear();
        }
    }

    public class DeviceInfoViewModel
    {
        public DeviceInfoViewModel(DeviceInstance device, EvcVerificationViewModel test)
        {
            SiteInfo = device.ItemGroup<SiteInformationItems>();
            Pressure = device.ItemGroup<PressureItems>();
            Temperature = device.ItemGroup<TemperatureItems>();
            SuperFactor = device.ItemGroup<SuperFactorItems>();
            Volume = device.ItemGroup<VolumeItems>();
            PulseOutput = device.ItemGroup<PulseOutputItems>();
        }

        public SiteInformationItems SiteInfo { get; protected set; }

        public PressureItems Pressure { get; protected set; }

        public TemperatureItems Temperature { get; protected set; }

        public SuperFactorItems SuperFactor { get; protected set; }

        public PulseOutputItems PulseOutput { get; protected set; }

        public VolumeItems Volume { get; protected set; }
    }
}