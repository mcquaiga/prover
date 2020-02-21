using System;
using System.Collections.Generic;
using System.Linq;
using Application.ViewModels.Corrections;
using Devices.Core.Items.ItemGroups;
using ReactiveUI.Fody.Helpers;

namespace Application.ViewModels.Volume
{
    public abstract class VolumeViewModel : VerificationViewModel
    {
        protected VolumeViewModel(VolumeItems startValues, VolumeItems endValues)
        {
            StartValues = startValues;
            EndValues = endValues;
        }

        public override Guid Id => Guid.Empty;
        [Reactive] public VolumeItems StartValues { get; set; }
        [Reactive] public VolumeItems EndValues { get; set; }
        [Reactive] public decimal AppliedInput { get; set; }

        [Reactive] public CorrectedVolumeTestViewModel Corrected { get; set; }
        [Reactive] public UncorrectedVolumeTestViewModel Uncorrected { get; set; }

        public virtual ICollection<VerificationViewModel> AllTests()
        {
            var tests = GetSpecificTests().ToList();
            
            tests.AddRange(new VerificationViewModel [] { Corrected, Uncorrected });

             return tests;
        }

        protected abstract ICollection<VerificationViewModel> GetSpecificTests();
    }
}


