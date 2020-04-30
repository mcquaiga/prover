using System.Collections.Generic;
using System.Linq;
using Devices.Core.Items.ItemGroups;
using Prover.Application.ViewModels.Corrections;

namespace Prover.Application.ViewModels.Volume.Rotary
{
    public class RotaryVolumeViewModel : VolumeViewModelBase
    {
        public RotaryVolumeViewModel(VolumeItems startValues, VolumeItems endValues) : base(startValues, endValues)
        {
        }

        public RotaryMeterTestViewModel RotaryMeterTest => AllTests().OfType<RotaryMeterTestViewModel>().FirstOrDefault();

        protected override ICollection<VerificationViewModel> GetSpecificTests() => AllTests();
    }
}