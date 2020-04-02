using System.Collections.Generic;
using System.Linq;
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

        public CorrectedVolumeTestViewModel Corrected => AllTests().OfType<CorrectedVolumeTestViewModel>().FirstOrDefault();
        public UncorrectedVolumeTestViewModel Uncorrected => AllTests().OfType<UncorrectedVolumeTestViewModel>().FirstOrDefault();
        public RotaryMeterTestViewModel RotaryMeterTest => AllTests().OfType<RotaryMeterTestViewModel>().FirstOrDefault();

        protected override ICollection<VerificationViewModel> GetSpecificTests() => AllTests();
    }
}