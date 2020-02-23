using System;
using Devices.Core.Items.ItemGroups;
using Domain.Interfaces;
using Shared.Extensions;

namespace Domain.EvcVerifications.Verifications
{
    public class PulseOutputChannelTest : VerificationTestEntity<PulseOutputItems>
    {
        public bool HasPassed() => Math.Abs(ExpectedValue - ActualValue).IsBetween(Global.PULSE_VARIANCE_THRESHOLD);
        
    }
}