using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Prover.Application.ViewModels.Corrections;
using Prover.Application.ViewModels.Volume;

namespace Prover.Application.ViewModels.Factories.Volume
{
    public partial class VolumeViewModelFactory
    {
        private void MechanicalVolumeFactory(DeviceInstance device, EvcVerificationViewModel viewModel,
            VerificationTestPointViewModel testPoint)
        {
            var vm = new MechanicalVolumeViewModel(_startVolumeItems, _endVolumeItems);
            
            vm.AddVerificationTest(CreateUncorrectedVolumeTest(viewModel.DriveType));
            vm.AddVerificationTest(CreateCorrectedVolumeTest(viewModel.DriveType, vm.Uncorrected));
            
            CreatePulseOutputTests(device, vm.Uncorrected, vm.Corrected);

            vm.AddVerificationTest(new EnergyVolumeTestViewModel(1m, _startVolumeItems, _endVolumeItems));
            
            testPoint.AddTest(vm);
        }
    }

    public class MechanicalVolumeViewModel : VolumeViewModelBase
    {
        public MechanicalVolumeViewModel(VolumeItems startVolumeItems, VolumeItems endVolumeItems)
        : base(startVolumeItems, endVolumeItems)
        {
        }

        public EnergyVolumeTestViewModel Energy => AllTests().OfType<EnergyVolumeTestViewModel>().FirstOrDefault();

        /// <inheritdoc />
        protected override ICollection<VerificationViewModel> GetSpecificTests() => throw new System.NotImplementedException();
    }

    public class EnergyVolumeTestViewModel  : VolumeTestRunViewModelBase
    {
        /// <inheritdoc />
        public EnergyVolumeTestViewModel(decimal passTolerance, VolumeItems startValues, VolumeItems endValues) : base(passTolerance, startValues, endValues)
        {
        }
    }
}
