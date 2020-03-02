using Devices.Core.Items.ItemGroups;
using Prover.Application.ViewModels.Corrections;
using Prover.Domain.EvcVerifications.Verifications.Volume.InputTypes;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels.Volume
{
    public abstract class VolumeTestRunViewModelBase : VarianceTestViewModel
    {
        protected VolumeTestRunViewModelBase(decimal passTolerance, IVolumeInputType driveType, VolumeItems startValues, VolumeItems endValues) : base(passTolerance)
        {
            DriveType = driveType;
            StartValues = startValues;
            EndValues = endValues;
        }

        [Reactive] public VolumeItems StartValues { get; set; }
        [Reactive] public VolumeItems EndValues { get; set; }

        [Reactive] public IVolumeInputType DriveType { get; set; }

        
    }
}