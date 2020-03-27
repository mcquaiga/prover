using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Devices.Core.Interfaces;
using Prover.Application.ViewModels.Corrections;
using Prover.Application.ViewModels.Volume;
using Prover.Domain.EvcVerifications.Verifications.Volume.InputTypes;
using Prover.Shared;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels
{
    public class EvcVerificationViewModel : ViewModelWithIdBase, IDisposable, IRoutableViewModel
    {
        [Reactive] public DeviceInstance Device { get; set; }

        [Reactive] public CompositionType CompositionType { get; set; }

        [Reactive] public IVolumeInputType DriveType { get; set; }

        [Reactive] public DateTime TestDateTime { get; set; } = DateTime.Now;

        [Reactive] public DateTime? ExportedDateTime { get; set; }

        [Reactive] public DateTime? ArchivedDateTime { get; set; }

        public ICollection<VerificationTestPointViewModel> Tests { get; set; } =
            new List<VerificationTestPointViewModel>();

        [Reactive]
        public ICollection<VerificationViewModel> OtherTests { get; set; } = new List<VerificationViewModel>();

        public DeviceInfoViewModel DeviceInfo { get; set; }

        public VolumeViewModelBase VolumeTest => Tests.FirstOrDefault(t => t.Volume != null)?.Volume;

        public string UrlPathSegment { get; }
        public IScreen HostScreen { get; }

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
}