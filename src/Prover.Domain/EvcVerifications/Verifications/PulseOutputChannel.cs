using System;
using Devices.Core.Items.ItemGroups;
using Prover.Shared.Extensions;

namespace Prover.Domain.EvcVerifications.Verifications
{
    public class PulseOutputChannelTest : VerificationTestEntity<PulseOutputItems>
    {
        public bool HasPassed() => Math.Abs(ExpectedValue - ActualValue).IsBetween(Global.PULSE_VARIANCE_THRESHOLD);
        
    }
}