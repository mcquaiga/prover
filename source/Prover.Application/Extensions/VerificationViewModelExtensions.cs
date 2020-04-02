using System.Collections.Generic;
using System.Linq;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Corrections;
using Prover.Application.ViewModels.Volume;

namespace Prover.Application.Extensions
{
    public static class VerificationViewModelExtensions
    {
        public static PressureFactorViewModel GetPressureTest(this VerificationTestPointViewModel testPoint)
        {
            return testPoint.VerificationTests.OfType<PressureFactorViewModel>().FirstOrDefault();
        }

        public static TemperatureFactorViewModel GetTemperatureTest(this VerificationTestPointViewModel testPoint) => testPoint.VerificationTests.OfType<TemperatureFactorViewModel>().FirstOrDefault();

        public static VolumeViewModelBase GetVolumeTest(this VerificationTestPointViewModel testPoint) => testPoint.VerificationTests.OfType<VolumeViewModelBase>().FirstOrDefault();

        public static SuperFactorViewModel GetSuperFactorTest(this VerificationTestPointViewModel testPoint) => testPoint.VerificationTests.OfType<SuperFactorViewModel>().FirstOrDefault();

        public static VolumeViewModelBase GetVolumeTest(this EvcVerificationViewModel test) => test.VerificationTests.OfType<VerificationTestPointViewModel>().FirstOrDefault(t => t.Volume != null)?.Volume;

        public static ICollection<VerificationViewModel> GetCorrectionTests(this VerificationTestPointViewModel test)
        {
            return test.VerificationTests.Where(testType =>
            {
                var memberInfo = testType.GetType().BaseType;
                return memberInfo != null && (memberInfo.IsGenericType &&
                                              memberInfo.GetGenericTypeDefinition() ==
                                              typeof(CorrectionTestViewModel<>));
            }).ToList();
        }

        public static VolumeTestRunViewModelBase GetUncorrectedTest(this VolumeViewModelBase volumeTest)
        {
            return (VolumeTestRunViewModelBase) volumeTest.AllTests().FirstOrDefault();
        }

        public static string TestDateTimePretty(this EvcVerificationViewModel test)
        {
            return $"{test.TestDateTime:g}";
        }
    }
}