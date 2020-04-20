using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items.DriveTypes;
using Prover.Application.ViewModels.Corrections;
using Prover.Application.ViewModels.Volume.Mechanical;

namespace Prover.Application.ViewModels.Factories.Volume
{
    public partial class VolumeViewModelFactory
    {
        private void MechanicalVolumeFactory(DeviceInstance device, EvcVerificationViewModel viewModel,
            VerificationTestPointViewModel testPoint)
        {
            var vm = new MechanicalVolumeViewModel(_startVolumeItems, _endVolumeItems);
            vm.DriveType = viewModel.DriveType;

            vm.AddVerificationTest(CreateUncorrectedVolumeTest(viewModel.DriveType));
            vm.AddVerificationTest(CreateCorrectedVolumeTest(viewModel.DriveType, vm.Uncorrected));
            
            CreatePulseOutputTests(device, vm.Uncorrected, vm.Corrected);

            vm.AddVerificationTest(new EnergyVolumeTestViewModel(device.CreateItemGroup<EnergyItems>(), device.CreateItemGroup<EnergyItems>()));
            
            testPoint.AddTest(vm);
        }
    }
}
