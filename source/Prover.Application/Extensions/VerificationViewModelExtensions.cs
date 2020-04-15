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
        public static ICollection<VerificationViewModel> GetCorrectionTests(this VerificationTestPointViewModel test)
        {
            return test.VerificationTests.Where(testType =>
            {
                var memberInfo = testType.GetType().BaseType;
                return memberInfo != null && memberInfo.IsGenericType && memberInfo.GetGenericTypeDefinition() ==
                        typeof(CorrectionTestViewModel<>);
            }).ToList();
        }

        

        public static PressureFactorViewModel GetPressureTest(this VerificationTestPointViewModel testPoint) => testPoint.VerificationTests.OfType<PressureFactorViewModel>().FirstOrDefault();

        public static SuperFactorViewModel GetSuperFactorTest(this VerificationTestPointViewModel testPoint) => testPoint.VerificationTests.OfType<SuperFactorViewModel>().FirstOrDefault();

        public static TemperatureFactorViewModel GetTemperatureTest(this VerificationTestPointViewModel testPoint) => testPoint.VerificationTests.OfType<TemperatureFactorViewModel>().FirstOrDefault();

        public static VolumeTestRunViewModelBase GetUncorrectedTest(this VolumeViewModelBase volumeTest) => (VolumeTestRunViewModelBase) volumeTest.AllTests().FirstOrDefault();

        public static VolumeViewModelBase GetVolumeTest(this VerificationTestPointViewModel testPoint) => testPoint.VerificationTests.OfType<VolumeViewModelBase>().FirstOrDefault();

        public static VolumeViewModelBase GetVolumeTest(this EvcVerificationViewModel test) => test.VerificationTests.OfType<VerificationTestPointViewModel>().FirstOrDefault(t => t.Volume != null)?.Volume;

        public static void SetItems<T>(this EvcVerificationViewModel verification, DeviceInstance device, int testNumber, Dictionary<string, string> valuesDictionary)
                where T : ItemGroup, ICorrectionFactor
        {
            var itemValues = device.DeviceType.ToItemValues(valuesDictionary);
            verification.SetItems<T>(device, testNumber, itemValues);
        }

        public static void SetItems<T>(this EvcVerificationViewModel verification, DeviceInstance device, int testNumber,
                IEnumerable<ItemValue> itemValues)
                where T : ItemGroup, ICorrectionFactor
        {
            var testPoint = verification.VerificationTests.OfType<VerificationTestPointViewModel>().FirstOrDefault(v => v.TestNumber == testNumber);

            var testOfT = testPoint?.VerificationTests.OfType<CorrectionTestViewModel<T>>().FirstOrDefault();
            if (testOfT != null)
                    //itemValues = device.CreateGroupItemValues<T>(itemValues);
                testOfT.Items = device.CreateItemGroup<T>(itemValues);
        }

        public static string TestDateTimePretty(this EvcVerificationViewModel test) => $"{test.TestDateTime:g}";
    }
}