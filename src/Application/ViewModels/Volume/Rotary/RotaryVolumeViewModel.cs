using System.Collections.Generic;
using Application.ViewModels.Corrections;
using Devices.Core.Items.ItemGroups;

namespace Application.ViewModels.Volume.Rotary
{
    public class RotaryVolumeViewModel : VolumeViewModelBase
    {
        public RotaryVolumeViewModel(VolumeItems startValues, VolumeItems endValues) : base(startValues, endValues)
        {
        }

        public RotaryMeterTestViewModel RotaryMeterTest { get; set; }

        protected override ICollection<VerificationViewModel> GetSpecificTests()
        {
            return new List<VerificationViewModel>{ RotaryMeterTest };
        }
    }
}