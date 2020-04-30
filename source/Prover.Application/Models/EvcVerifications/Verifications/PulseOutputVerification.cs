using Devices.Core.Items.ItemGroups;
using Newtonsoft.Json;
using Prover.Calculations;
using Prover.Shared.Extensions;

namespace Prover.Application.Models.EvcVerifications.Verifications
{
	public interface IPulseOutputVerification
	{
		PulseOutputVerification PulseOutputTest { get; set; }
	}

	public class PulseOutputVerification : VerificationTestEntity<PulseOutputItems.ChannelItems>
	{
		private readonly decimal? _multiplier;

		/*
		public bool HasPassed() => Math.Abs(ExpectedValue - ActualValue).IsBetween(Global.PULSE_VARIANCE_THRESHOLD);
*/
		public PulseOutputVerification()
		{
		}

		[JsonConstructor]
		public PulseOutputVerification(PulseOutputItems.ChannelItems items, decimal actualValue, decimal expectedValue, decimal percentError, bool verified)
				: base(items, expectedValue, actualValue, percentError, verified)
		{
			//_multiplier = (Items as IVolumeUnits)?.Units.Multiplier ?? multiplier;
			ActualValue = actualValue;
		}

		public decimal TotalVolume { get; set; }

		public void Calculate()
		{
			ExpectedValue = VolumeCalculator.PulseCount(TotalVolume, _multiplier);
			PercentError = Calculators.PercentDeviation(ExpectedValue, ActualValue);
			Verified = PercentError.IsBetween(Tolerances.PULSE_VARIANCE_THRESHOLD);
		}
	}
}