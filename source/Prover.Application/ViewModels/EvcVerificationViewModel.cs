using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Devices.Core.Interfaces;
using DynamicData;
using Prover.Application.ViewModels.Corrections;
using Prover.Application.ViewModels.Volume;
using Prover.Domain.EvcVerifications.Verifications.Volume.InputTypes;
using Prover.Shared;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels
{
    public class EvcVerificationViewModel : VerificationViewModel
    {
        [Reactive] public DeviceInstance Device { get; set; }

        [Reactive] public CompositionType CompositionType { get; set; }

        [Reactive] public IVolumeInputType DriveType { get; set; }

        [Reactive] public DateTime TestDateTime { get; set; } = DateTime.Now;

        [Reactive] public DateTime? ExportedDateTime { get; set; }

        [Reactive] public DateTime? ArchivedDateTime { get; set; }

        [Reactive] public string JobId { get; set; }

        [Reactive] public string EmployeeId { get; set; }


        public ICollection<VerificationViewModel> VerificationTests { get; set; } =
            new List<VerificationViewModel>();

        public SiteInformationViewModel DeviceInfo { get; set; }

        public VolumeViewModelBase VolumeTest => VerificationTests.OfType<VerificationTestPointViewModel>().FirstOrDefault(t => t.Volume != null)?.Volume;
        
        public void Initialize()
        {
            DeviceInfo = new SiteInformationViewModel(Device, this);
        }

        public void SetupVerifiedObserver()
        {
            VerificationTests.AsObservableChangeSet()
                .AutoRefresh(model => model.Verified) 
                .ToCollection()                    
                .Select(x => x.All(y => y.Verified))
                //.Subscribe(v => Verified = v);
                .ToPropertyEx(this, x => x.Verified);
        }

        protected override void Disposing()
        {
            VerificationTests.ForEach(t => t.DisposeWith(Cleanup));
            //VerificationTests.ForEach(t => t.DisposeWith(Cleanup));
            VerificationTests.Clear();
        }

        //public string UrlPathSegment { get; } = "EvcVerificationTest";
        //public IScreen HostScreen { get; }
    }
}