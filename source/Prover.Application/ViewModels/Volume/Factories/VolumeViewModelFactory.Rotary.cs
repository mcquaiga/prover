using Devices.Core.Interfaces;
using Devices.Core.Items.DriveTypes;
using Prover.Application.ViewModels.Volume.Rotary;

namespace Prover.Application.ViewModels.Volume.Factories
{
    public partial class VolumeViewModelFactory
    {
        private void CreateRotaryVolume(DeviceInstance device, 
            EvcVerificationViewModel verificationViewModel,
            VerificationTestPointViewModel testPoint)
        {
            var rotaryViewModel = new RotaryVolumeViewModel(_startVolumeItems, _endVolumeItems);
            rotaryViewModel.AddVerificationTest(CreateUncorrectedVolumeTest(verificationViewModel.DriveType));
            rotaryViewModel.AddVerificationTest(CreateCorrectedVolumeTest(verificationViewModel.DriveType, rotaryViewModel.Uncorrected));
            rotaryViewModel.DriveType = verificationViewModel.DriveType;

            CreatePulseOutputTests(device, rotaryViewModel.Uncorrected, rotaryViewModel.Corrected);
            
            var rotaryMeterTest = new RotaryMeterTestViewModel(device.ItemGroup<RotaryMeterItems>());
            rotaryViewModel.AddVerificationTest(rotaryMeterTest);

            testPoint.AddTest(rotaryViewModel);
        }
    }
}