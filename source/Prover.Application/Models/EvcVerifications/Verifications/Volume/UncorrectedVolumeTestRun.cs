using Devices.Core.Items.ItemGroups;
using Prover.Application.Models.EvcVerifications.Verifications.Volume.InputTypes;
using Prover.Calculations;

namespace Prover.Application.Models.EvcVerifications.Verifications.Volume
{
	public class UncorrectedVolumeTestRun : VerificationTestEntity<VolumeItems, VolumeItems>, IPulseOutputVerification
	{
		private UncorrectedVolumeTestRun()
		{
		}

		public UncorrectedVolumeTestRun(VolumeItems startValues, VolumeItems endValues, IVolumeInputType driveType, decimal appliedInput) : base(startValues, endValues, 0, 0, 100m, false)
		{
			AppliedInput = appliedInput;

			Calculate(driveType);
		}

		public decimal AppliedInput { get; set; }

		public PulseOutputVerification PulseOutputTest { get; set; }

		public void Calculate(IVolumeInputType driveType)
		{
			ActualValue = VolumeCalculator.TotalVolume(StartValues.UncorrectedReading, EndValues.UncorrectedReading, StartValues.UncorrectedMultiplier);
			ExpectedValue = driveType.UnCorrectedInputVolume(AppliedInput);
		}
	}
}