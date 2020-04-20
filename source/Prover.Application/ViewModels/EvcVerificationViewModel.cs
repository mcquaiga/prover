using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Devices.Core.Interfaces;
using Prover.Application.Models.EvcVerifications.Verifications.Volume.InputTypes;
using Prover.Application.ViewModels.Corrections;
using Prover.Application.ViewModels.Volume;
using Prover.Shared;
using Prover.Shared.Interfaces;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels
{
    public class EvcVerificationViewModel : VerificationViewModel
    {
        public EvcVerificationViewModel()
        {
        }

        private EvcVerificationViewModel(bool verified)
        {
        }

        [Reactive] public DeviceInstance Device { get; set; }

        [Reactive] public CompositionType CompositionType { get; set; }

        [Reactive] public IVolumeInputType DriveType { get; set; }

        [Reactive] public DateTime TestDateTime { get; set; }

        [Reactive] public DateTime? ExportedDateTime { get; set; }

        [Reactive] public DateTime? SubmittedDateTime { get; set; }

        [Reactive] public DateTime? ArchivedDateTime { get; set; }

        [Reactive] public string JobId { get; set; }

        [Reactive] public string EmployeeId { get; set; }

        [Reactive] public string EmployeeName { get; set; }

        public ICollection<VerificationViewModel> VerificationTests { get; set; } = new List<VerificationViewModel>();

        public SiteInformationViewModel DeviceInfo { get; set; }

        public VolumeViewModelBase VolumeTest => VerificationTests.OfType<VerificationTestPointViewModel>().FirstOrDefault(t => t.Volume != null)?.Volume;

        public void Initialize(ICollection<VerificationViewModel> verificationTests, ILoginService loginService = null)
        {
            DeviceInfo = new SiteInformationViewModel(Device, this, loginService);

            VerificationTests.Clear();
            VerificationTests.AddRange(verificationTests.ToArray());

            RegisterVerificationsForVerified(VerificationTests);
        }

        protected override void Dispose(bool isDisposing)
        {
            VerificationTests.ForEach(t => t.DisposeWith(Cleanup));
            VerificationTests.Clear();
        }
    }
}