using Devices.Core.Items.ItemGroups;

namespace Prover.Application.Models.EvcVerifications.Verifications
{
    public interface IPulseOutputVerification
    {
        PulseOutputVerification PulseOutputTest { get; set; }
    }

    public class PulseOutputVerification : VerificationTestEntity<PulseOutputItems.ChannelItems>
    {
/*
        public bool HasPassed() => Math.Abs(ExpectedValue - ActualValue).IsBetween(Global.PULSE_VARIANCE_THRESHOLD);
*/
        public PulseOutputVerification()
        {

        }

        public PulseOutputVerification(PulseOutputItems.ChannelItems items, decimal expectedValue, decimal actualValue,
            decimal percentError) : base(items, expectedValue, actualValue, percentError)
        {
        }
    }
}