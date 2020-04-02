using Devices.Core.Interfaces;
using Devices.Core.Items.DriveTypes;
using Prover.Application.ViewModels.Volume.Rotary;

namespace Prover.Application.ViewModels.Volume.Factories
{
    public partial class VolumeViewModelFactory
    {
        private void CreateRotaryVolume(DeviceInstance device, EvcVerificationViewModel viewModel,
            VerificationTestPointViewModel testPoint)
        {
            var vm = new RotaryVolumeViewModel(_startVolumeItems, _endVolumeItems)
            {
                Uncorrected = CreateUncorrectedVolumeTest(viewModel.DriveType)
            };
            vm.Corrected = CreateCorrectedVolumeTest(viewModel.DriveType, vm.Uncorrected);
            vm.DriveType = viewModel.DriveType;

            CreatePulseOutputTests(device, vm.Uncorrected, vm.Corrected);

            var rotary = new RotaryMeterTestViewModel(device.ItemGroup<RotaryMeterItems>());
            vm.RotaryMeterTest = rotary;
            testPoint.VerificationTests.Add(vm);
        }
    }
}