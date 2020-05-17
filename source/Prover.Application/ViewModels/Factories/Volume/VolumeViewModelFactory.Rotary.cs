using Devices.Core.Interfaces;
using Devices.Core.Items.DriveTypes;
using Prover.Application.ViewModels.Volume;
using Prover.Application.ViewModels.Volume.Rotary;

namespace Prover.Application.ViewModels.Factories.Volume
{
	public partial class VolumeViewModelFactory
	{
		private void CreateRotaryVolume(DeviceInstance device,
			EvcVerificationViewModel verificationViewModel,
			VerificationTestPointViewModel testPoint)
		{
			var rotaryViewModel = new RotaryVolumeViewModel(_startVolumeItems, _endVolumeItems);

			rotaryViewModel.AddVerificationTest((UncorrectedVolumeTestViewModel)new RotaryUncorrectedVolumeTestViewModel(device.ItemGroup<RotaryMeterItems>(), _startVolumeItems, _endVolumeItems));
			rotaryViewModel.AddVerificationTest(CreateCorrectedVolumeTest(rotaryViewModel.Uncorrected));

			CreatePulseOutputTests(device, (UncorrectedVolumeTestViewModel)rotaryViewModel.Uncorrected, rotaryViewModel.Corrected);

			var rotaryMeterTest = new RotaryMeterTestViewModel(device.ItemGroup<RotaryMeterItems>());
			rotaryViewModel.AddVerificationTest(rotaryMeterTest);

			testPoint.AddTest(rotaryViewModel);
		}

	}
}