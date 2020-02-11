using System.Linq;
using System.Threading.Tasks;
using Application.ViewModels;
using Application.ViewModels.Services;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Items;
using Domain.EvcVerifications;
using Domain.EvcVerifications.Builders;
using Domain.EvcVerifications.CorrectionTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tests.Shared;

namespace Tests.Domain.EvcVerifications.Builders
{
    [TestClass()]
    public class EvcVerificationBuilderTests
    {
        private Mock<DeviceInstance> _instance;
        private DeviceInstance _device;
        private DeviceType _deviceType;
        private DeviceRepository _repo;

        [TestInitialize]
        public async Task Init()
        {
            _instance = new Mock<DeviceInstance>();
            _repo = await Devices.Devices.Repository();
            _deviceType = _repo.GetByName("Mini-Max");

            var items = ItemFiles.MiniMaxItemFile;
            _device = _deviceType.CreateInstance(items);
        }

        [TestMethod()]
        public void EvcVerificationBuilderTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateNewTest()
        {
            var test = new EvcVerificationBuilder(_device);

            test.TestPointFactory()
                .CreateNew(0, _device.Values, _device.Values)
                .BuildTemperatureTest(_device.ItemGroup<ITemperatureItems>(), 32)
                .BuildPressureTest(_device.ItemGroup<IPressureItems>(), 80)
                .BuildSuperFactorTest(_device.ItemGroup<ISuperFactorItems>())
                .BuildVolumeTest(_device.ItemGroup<IVolumeItems>(), _device.ItemGroup<IVolumeItems>(), 14000)
                .Commit();

            test.TestPointFactory()
                .CreateNew(1, _device.Values, _device.Values)
                .BuildTemperatureTest(_device.ItemGroup<ITemperatureItems>(), 60)
                .BuildPressureTest(_device.ItemGroup<IPressureItems>(), 50)
                .BuildSuperFactorTest(_device.ItemGroup<ISuperFactorItems>())
                .Commit();

            test.TestPointFactory()
                .CreateNew(2, _device.Values, _device.Values)
                .BuildTemperatureTest(_device.ItemGroup<ITemperatureItems>(), 90)
                .BuildPressureTest(_device.ItemGroup<IPressureItems>(), 20)
                .BuildSuperFactorTest(_device.ItemGroup<ISuperFactorItems>())
                .Commit();

            var verification = test.GetEvcVerification();

            Assert.IsNotNull(verification);

            //var tempTest = verification.GetTest<VerificationTestPoint>(t => t.TestNumber == 0)
            //    .GetTest<CorrectionTestCalculatorDecorator>(t => t.TestType == CorrectionFactorTestType.Temperature);

            var lowTemp = _deviceType.ToItemValuesEnumerable(ItemFiles.TempLowItems);
            var ti = _device.ItemGroup<ITemperatureItems>(lowTemp);

            //CorrectionTest.Update<ITemperatureItems>(tempTest, ti, ti.Factor)
            var tempVm = new TemperatureFactorViewModel(ti, 32);
            var factor = tempVm.FactorTest.ExpectedValue;
            
            var midTemp = _deviceType.ToItemValuesEnumerable(ItemFiles.TempMidItems);
            ti = _device.ItemGroup<ITemperatureItems>(midTemp);

            tempVm.Gauge = 60;
            tempVm.Update(ti);

            var factor2 = tempVm.FactorTest.ExpectedValue;

            Assert.AreNotEqual(factor, factor2);
        }

        [TestMethod()]
        public async Task GetCurrentTestPointBuilderTest()
        {
            var service = new EvcVerificationViewModelService();
            var vm = await service.CreateNewVerificationTest(_device);

            Assert.IsNotNull(vm);
        }

        [TestMethod()]
        public void GetEvcVerificationTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void NewTestPointTest()
        {
            Assert.Fail();
        }
    }
}