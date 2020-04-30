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

        public static void UpdateValues(this VerificationTestPointViewModel test, ICollection<ItemValue> values, DeviceInstance device)
        {
            //foreach (var correction in test.GetCorrectionTests())
            //{
            //    var itemType = correction.GetProperty(nameof(CorrectionTestViewModel<IItemGroup>.Items));

            //    itemType?.SetValue(correction, device.DeviceType.GetGroupValues(values, itemType.PropertyType));
            //}
            values = device.CombineValuesWithItemFile(values).ToList();
            foreach (var verificationTest in test.GetCorrectionTests())
            {
                var itemType = verificationTest.GetProperty(nameof(CorrectionTestViewModel<IItemGroup>.Items));

                itemType?.SetValue(verificationTest, device.DeviceType.GetGroupValues(values, itemType.PropertyType));
            }
        }

        public static void UpdateValues(this VerificationTestPointViewModel test, ICollection<ItemValue> startValues, ICollection<ItemValue> endValues, DeviceInstance device)
        {
            //foreach (var correction in test.GetCorrectionTests())
            //{
            //    var itemType = correction.GetProperty(nameof(CorrectionTestViewModel<IItemGroup>.Items));

            //    itemType?.SetValue(correction, device.DeviceType.GetGroupValues(values, itemType.PropertyType));
            //}

            if (test.Volume == null) return;

            startValues = device.CombineValuesWithItemFile(startValues)
                                .ToList();

            endValues = device.CombineValuesWithItemFile(endValues)
                              .ToList();

            foreach (var verificationTest in test.Volume?.AllTests().OfType<IDeviceStartAndEndValues<VolumeItems>>())
            {
                var itemType = verificationTest.GetProperty(nameof(IDeviceStartAndEndValues<VolumeItems>.StartValues));
                itemType?.SetValue(verificationTest, device.DeviceType.GetGroupValues(startValues, itemType.PropertyType));

                itemType = verificationTest.GetProperty(nameof(IDeviceStartAndEndValues<VolumeItems>.EndValues));
                itemType?.SetValue(verificationTest, device.DeviceType.GetGroupValues(endValues, itemType.PropertyType));
            }
        }

        public static string TestDateTimePretty(this EvcVerificationViewModel test) => $"{test.TestDateTime:g}";
    }
}