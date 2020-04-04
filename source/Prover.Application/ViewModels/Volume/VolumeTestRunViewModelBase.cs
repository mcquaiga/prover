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

        [Reactive] public decimal StartReading { get; set; }
        [Reactive] public decimal EndReading { get; set; }
        [Reactive] public decimal Multiplier { get; set; }

        public virtual PulseOutputTestViewModel PulseOutputTest { get; set; }
    }
}