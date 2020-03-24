using System.Collections.Generic;
using System.Linq;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Prover.Domain.EvcVerifications.Verifications;
using Prover.Domain.EvcVerifications.Verifications.Volume;

namespace Prover.Domain.EvcVerifications
{
    public static class EvcVerificationExtensions
    {
        public static T GetCorrectionTest<T>(this VerificationTestPoint vtp)
            where T : VerificationTestEntity<ItemGroup> => vtp.GetTest<T>();

        public static T GetVerificationTest<T>(this VerificationTestPoint vtp)
            where T : VerificationEntity => vtp.GetTest<T>();

        public static VerificationTestPoint GetVolumeTest(this EvcVerificationTest evcVerification)
        {
            return evcVerification.Tests
                .Where(t => t.GetType() == typeof(VerificationTestPoint))
                .Select(t => (VerificationTestPoint) t)
                .FirstOrDefault(ct => ct.GetTest<CorrectedVolumeTestRun>() != null);
        }

        public static bool HasVolume(this VerificationTestPoint point)
        {
            return point.Tests.Any(t => t.GetType() == typeof(CorrectedVolumeTestRun));
        }

        public static void UpdateValues(this VerificationTestPoint vtp, IEnumerable<ItemValue> beforeValues,
            IEnumerable<ItemValue> afterValues, decimal appliedInput)
        {
        }
    }
}