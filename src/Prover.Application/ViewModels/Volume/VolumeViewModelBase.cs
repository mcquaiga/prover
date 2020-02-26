using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Prover.Application.ViewModels.Corrections;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels.Volume
{
    public abstract class VolumeViewModelBase : VerificationViewModel
    {
        protected VolumeViewModelBase(VolumeItems startValues, VolumeItems endValues)
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

        public virtual void UpdateStartValues(IEnumerable<ItemValue> values)
        {

        }

        public virtual void UpdateEndValues(IEnumerable<ItemValue> values)
        {

        }

        protected abstract ICollection<VerificationViewModel> GetSpecificTests();
    }
}


