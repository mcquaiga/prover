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
            return testPoint.TestsCollection.OfType<PressureFactorViewModel>().FirstOrDefault();
        }

        public static TemperatureFactorViewModel GetTemperatureTest(this VerificationTestPointViewModel testPoint) => testPoint.TestsCollection.OfType<TemperatureFactorViewModel>().FirstOrDefault();

        public static VolumeViewModelBase GetVolumeTest(this VerificationTestPointViewModel testPoint) => testPoint.TestsCollection.OfType<VolumeViewModelBase>().FirstOrDefault();

        public static SuperFactorViewModel GetSuperFactorTest(this VerificationTestPointViewModel testPoint) => testPoint.TestsCollection.OfType<SuperFactorViewModel>().FirstOrDefault();

        public static VolumeViewModelBase GetVolumeTest(this EvcVerificationViewModel test) => test.Tests.FirstOrDefault(t => t.Volume != null)?.Volume;

        public static ICollection<VerificationViewModel> GetCorrectionTests(this VerificationTestPointViewModel test)
        {
            return test.TestsCollection.Where(testType =>
            {
                var memberInfo = testType.GetType().BaseType;
                return memberInfo != null && (memberInfo.IsGenericType &&
                                              memberInfo.GetGenericTypeDefinition() ==
                                              typeof(CorrectionTestViewModel<>));
            }).ToList();
        }
    }
}