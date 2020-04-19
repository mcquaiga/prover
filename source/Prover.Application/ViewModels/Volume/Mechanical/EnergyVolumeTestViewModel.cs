using Devices.Core.Items.ItemGroups;

namespace Prover.Application.ViewModels.Volume.Mechanical
{
    public class EnergyVolumeTestViewModel  : VolumeTestRunViewModelBase
    {
        /// <inheritdoc />
        public EnergyVolumeTestViewModel(decimal passTolerance, VolumeItems startValues, VolumeItems endValues) : base(passTolerance, startValues, endValues)
        {
        }
    }
}