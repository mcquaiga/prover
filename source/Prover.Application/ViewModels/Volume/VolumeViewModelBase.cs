﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Devices.Core.Items.ItemGroups;
using Prover.Application.ViewModels.Corrections;
using Prover.Domain.EvcVerifications.Verifications.Volume.InputTypes;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels.Volume
{
    public abstract class VolumeViewModelBase : VerificationViewModel
    {
        private ICollection<VerificationViewModel> _allTests = new List<VerificationViewModel>();

        protected VolumeViewModelBase(VolumeItems startValues, VolumeItems endValues)
        {
            StartValues = startValues;
            EndValues = endValues;

            Id = Guid.Empty;
        }

        [Reactive] public VolumeItems StartValues { get; set; }
        [Reactive] public VolumeItems EndValues { get; set; }

        //[Reactive] public decimal AppliedInput { get; set; }
        //[Reactive] public CorrectedVolumeTestViewModel Corrected { get; set; }
        //[Reactive] public UncorrectedVolumeTestViewModel Uncorrected { get; set; }

        [Reactive] public IVolumeInputType DriveType { get; set; }

        public virtual ICollection<VerificationViewModel> AllTests() => GetSpecificTests().ToList();

        protected override void Disposing()
        {
            AllTests().ForEach(t => t.DisposeWith(Cleanup));
        }


        protected abstract ICollection<VerificationViewModel> GetSpecificTests();
    }
}