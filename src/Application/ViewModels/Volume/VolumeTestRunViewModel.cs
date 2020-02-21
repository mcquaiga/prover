using Application.ViewModels.Corrections;
using Devices.Core.Items.ItemGroups;
using Domain.EvcVerifications.Verifications.Volume.InputTypes;
using ReactiveUI.Fody.Helpers;

namespace Application.ViewModels.Volume
{
    public abstract class VolumeTestRunViewModel : VarianceTestViewModel
    {
        protected VolumeTestRunViewModel(decimal passTolerance, IVolumeInputType driveType, VolumeItems startValues, VolumeItems endValues) : base(passTolerance)
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