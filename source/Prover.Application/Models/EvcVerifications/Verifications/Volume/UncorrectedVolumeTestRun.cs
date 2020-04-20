using Devices.Core.Items.ItemGroups;

namespace Prover.Application.Models.EvcVerifications.Verifications.Volume
{
    public class UncorrectedVolumeTestRun : VerificationTestEntity<VolumeItems, VolumeItems>, IPulseOutputVerification
    {
        private UncorrectedVolumeTestRun()
        {
        }

        public UncorrectedVolumeTestRun(VolumeItems startValues, VolumeItems endValues, decimal expectedValue,
            decimal actualValue, decimal percentError, bool verified, decimal appliedInput)
            : base(startValues, endValues, expectedValue, actualValue, percentError, verified) =>
            AppliedInput = appliedInput;

        public decimal AppliedInput { get; set; }

        public PulseOutputVerification PulseOutputTest { get; set; }
    }
}