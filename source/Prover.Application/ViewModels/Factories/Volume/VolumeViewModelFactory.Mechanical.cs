using Devices.Core.Interfaces;
using Devices.Core.Items.DriveTypes;
using Prover.Application.ViewModels.Volume;
using Prover.Application.ViewModels.Volume.Mechanical;
using Prover.Shared;

namespace Prover.Application.ViewModels.Factories.Volume
{
    public partial class VolumeViewModelFactory
    {

        private void MechanicalVolumeFactory(DeviceInstance device, EvcVerificationViewModel viewModel,
            VerificationTestPointViewModel testPoint)
        {
            var vm = new MechanicalVolumeViewModel(_startVolumeItems, _endVolumeItems);

            vm.AddVerificationTest(CreateUncorrectedVolumeTest(VolumeInputType.Mechanical));
            vm.AddVerificationTest(CreateCorrectedVolumeTest(vm.Uncorrected));

            CreatePulseOutputTests(device, (UncorrectedVolumeTestViewModel)vm.Uncorrected, vm.Corrected);

            vm.AddVerificationTest(new EnergyVolumeTestViewModel(device.CreateItemGroup<EnergyItems>(), device.CreateItemGroup<EnergyItems>(), vm.Corrected));

            testPoint.AddTest(vm);
        }

    }
}
