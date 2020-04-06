using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Corrections;
using Prover.Application.ViewModels.Volume;

namespace Prover.Application.Extensions
{
    public static class VerificationExtensions
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

        public static void SetItems<T>(this EvcVerificationViewModel verification, DeviceType deviceType,  int testNumber, Dictionary<string, string> valuesDictionary)
            where T : ItemGroup, IHaveFactor
        {
            var itemValues = deviceType.ToItemValues(valuesDictionary);
            verification.SetItems<T>(deviceType, testNumber, itemValues);
        }

        public static void SetItems<T>(this EvcVerificationViewModel verification, DeviceType deviceType, int testNumber,
            IEnumerable<ItemValue> itemValues)
            where T : ItemGroup, IHaveFactor
        {
            var testPoint = verification.VerificationTests.OfType<VerificationTestPointViewModel>().FirstOrDefault(v => v.TestNumber == testNumber);
            if (testPoint == null) 
                return;

            var testOfT = testPoint.VerificationTests.OfType<CorrectionTestViewModel<T>>().FirstOrDefault();
            if (testOfT != null)
                testOfT.Items = deviceType.GetGroupValues<T>(itemValues);
        }

        public static string TestDateTimePretty(this EvcVerificationViewModel test)
        {
            return $"{test.TestDateTime:g}";
        }
    }
}