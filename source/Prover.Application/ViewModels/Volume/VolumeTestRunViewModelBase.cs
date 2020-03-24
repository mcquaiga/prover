using Devices.Core.Items.ItemGroups;
using Prover.Application.ViewModels.Corrections;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels.Volume
{
    public abstract class VolumeTestRunViewModelBase : VarianceTestViewModel
    {
        protected VolumeTestRunViewModelBase(decimal passTolerance, VolumeItems startValues, VolumeItems endValues) :
            base(passTolerance)
        {
            StartValues = startValues;
            EndValues = endValues;
        }

        [Reactive] public VolumeItems StartValues { get; set; }
        [Reactive] public VolumeItems EndValues { get; set; }

        public virtual PulseOutputTestViewModel PulseOutputTest { get; set; }
    }
}