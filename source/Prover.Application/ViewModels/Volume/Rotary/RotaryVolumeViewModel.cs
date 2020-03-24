using System.Collections.Generic;
using Devices.Core.Items.ItemGroups;
using Microsoft.Extensions.Logging;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.ViewModels.Corrections;
using Prover.Shared.Interfaces;

namespace Prover.Application.ViewModels.Volume.Rotary
{
    public class RotaryVolumeViewModel : VolumeViewModelBase
    {
        public RotaryVolumeViewModel(VolumeItems startValues, VolumeItems endValues) : base(startValues, endValues)
        {
        }

        public CorrectedVolumeTestViewModel Corrected { get; set; }
        public UncorrectedVolumeTestViewModel Uncorrected { get; set; }
        public RotaryMeterTestViewModel RotaryMeterTest { get; set; }

        protected override ICollection<VerificationViewModel> GetSpecificTests() =>
            new List<VerificationViewModel> {RotaryMeterTest, Corrected, Uncorrected};
    }
}