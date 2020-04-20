using System.Reactive.Linq;
using Devices.Core.Items.ItemGroups;
using Prover.Application.ViewModels.Corrections;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels.Volume
{
    public interface IDeviceStartAndEndValues<T>
     where T: ItemGroup, IItemGroup
    {
        T StartValues { get; set; }
        T EndValues { get; set; }
    }

    public abstract class VolumeTestRunViewModelBase : VarianceTestViewModel, IDeviceStartAndEndValues<VolumeItems>
        
    {
        protected VolumeTestRunViewModelBase(decimal passTolerance, VolumeItems startValues, VolumeItems endValues) :
            base(passTolerance)
        {
            StartValues = startValues;
            EndValues = endValues;

            VerifiedObservable = VerifiedObservable.CombineLatest(this.WhenAnyValue(x => x.PulseOutputTest.Verified), 
                (test, pulses) => test && pulses);
        }

        [Reactive] public VolumeItems StartValues { get; set; }
        [Reactive] public VolumeItems EndValues { get; set; }

        [Reactive] public decimal StartReading { get; set; }
        [Reactive] public decimal EndReading { get; set; }
        [Reactive] public decimal Multiplier { get; set; }

        public virtual PulseOutputTestViewModel PulseOutputTest { get; set; }
    }
}