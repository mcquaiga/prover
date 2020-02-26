using System;
using Devices.Core.Items.ItemGroups;
using Prover.Shared.Extensions;

namespace Prover.Domain.EvcVerifications.Verifications
{
    public class PulseOutputVerification : VerificationTestEntity<PulseOutputItems.ChannelItems>
    {

        public bool HasPassed() => Math.Abs(ExpectedValue - ActualValue).IsBetween(Global.PULSE_VARIANCE_THRESHOLD);

        public PulseOutputVerification(PulseOutputItems.ChannelItems items, decimal expectedValue, decimal actualValue, decimal percentError) : base(items, expectedValue, actualValue, percentError)
        {
        }
    }
}