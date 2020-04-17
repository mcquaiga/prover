using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using Devices.Core.Items.ItemGroups;
using Prover.Application.Models.EvcVerifications.Verifications.Volume.InputTypes;
using Prover.Application.ViewModels.Corrections;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels.Volume
{
    public abstract class VolumeViewModelBase : VerificationViewModel
    {
        private ICollection<VerificationViewModel> _allTests = new List<VerificationViewModel>();

        protected VolumeViewModelBase(VolumeItems startValues, VolumeItems endValues)
        {
            Id = Guid.Empty;

            StartValues = startValues;
            EndValues = endValues;
        }

        [Reactive] public VolumeItems StartValues { get; set; }
        [Reactive] public VolumeItems EndValues { get; set; }

        //public ReactiveCommand<Unit, Unit> StartTest { get; protected set; }
        //public ReactiveCommand<Unit, Unit> FinishTest { get; protected set; }

        public virtual IVolumeInputType DriveType { get; set; }

        public virtual ICollection<VerificationViewModel> AllTests() => _allTests;

        protected override void Disposing()
        {
            AllTests().ForEach(t => t.DisposeWith(Cleanup));
        }

        public void AddVerificationTest(VerificationViewModel verification)
        {
            _allTests.Add(verification);
            RegisterVerificationsForVerified(_allTests);
        }

        protected abstract ICollection<VerificationViewModel> GetSpecificTests();
    }
}