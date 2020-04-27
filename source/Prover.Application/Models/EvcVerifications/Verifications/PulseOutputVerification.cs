using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Prover.Calculations;

namespace Prover.Application.Models.EvcVerifications.Verifications
{
    public interface IPulseOutputVerification
    {
        PulseOutputVerification PulseOutputTest { get; set; }
    }

    public class PulseOutputVerification : VerificationTestEntity<PulseOutputItems.ChannelItems>
    {
        private decimal? _multiplier;

        /*
        public bool HasPassed() => Math.Abs(ExpectedValue - ActualValue).IsBetween(Global.PULSE_VARIANCE_THRESHOLD);
*/
        public PulseOutputVerification()
        {

        }

        public PulseOutputVerification(PulseOutputItems.ChannelItems items, decimal totalVolume, decimal actualValue, decimal? multiplier) : base(items, 0, 0, 100m)
        {
            _multiplier = (Items as IVolumeUnits)?.Units.Multiplier ?? multiplier;
            ActualValue = actualValue;

        }

        public void Calculate(decimal totalExpectedVolume)
        {
            ExpectedValue = VolumeCalculator.PulseCount(totalExpectedVolume, _multiplier);
            PercentError = Calculators.PercentDeviation(ExpectedValue, ActualValue);
        }
    }
}