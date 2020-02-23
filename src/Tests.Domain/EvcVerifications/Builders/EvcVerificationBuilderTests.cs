using System.Threading.Tasks;
using Application.ViewModels.Corrections;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
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
            _repo = await Devices.RepositoryFactory.CreateDefaultAsync();
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
            //var test = new EvcVerificationBuilder(_device);

            //test.TestPointFactory()
            //    .CreateNew(0, _device.Values, _device.Values)
            //    .BuildTemperatureTest(_device.ItemGroup<TemperatureItems>(), 32)
            //    .BuildPressureTest(_device.ItemGroup<IPressureItems>(), 80, 14.7m)
            //    .BuildSuperFactorTest(_device.ItemGroup<ISuperFactorItems>())
            //    .BuildVolumeTest(_device.ItemGroup<IVolumeItems>(), _device.ItemGroup<IVolumeItems>(), 14000)
            //    .Commit();

            //test.TestPointFactory()
            //    .CreateNew(1, _device.Values, _device.Values)
            //    .BuildTemperatureTest(_device.ItemGroup<TemperatureItems>(), 60)
            //    .BuildPressureTest(_device.ItemGroup<IPressureItems>(), 50, 14.7m)
            //    .BuildSuperFactorTest(_device.ItemGroup<ISuperFactorItems>())
            //    .Commit();

            //test.TestPointFactory()
            //    .CreateNew(2, _device.Values, _device.Values)
            //    .BuildTemperatureTest(_device.ItemGroup<TemperatureItems>(), 90)
            //    .BuildPressureTest(_device.ItemGroup<IPressureItems>(), 20, 14.7m)
            //    .BuildSuperFactorTest(_device.ItemGroup<ISuperFactorItems>())
            //    .Commit();

            //var verification = test.GetEvcVerification();

           // Assert.IsNotNull(verification);

            //var tempTest = verification.GetTest<VerificationTestPoint>(t => t.TestNumber == 0)
            //    .GetTest<CorrectionTestCalculatorDecorator>(t => t.TestType == CorrectionFactorTestType.Temperature);

            var lowTemp = _deviceType.ToItemValuesEnumerable(ItemFiles.TempLowItems);
            var ti = _device.ItemGroup<TemperatureItems>(lowTemp);

            //CorrectionTest.Update<TemperatureItems>(tempTest, ti, ti.Factor)
            var tempVm = new TemperatureFactorViewModel(ti, 32);
            var factor = tempVm.ExpectedValue;
            
            var midTemp = _deviceType.ToItemValuesEnumerable(ItemFiles.TempMidItems);
            ti = _device.ItemGroup<TemperatureItems>(midTemp);

            tempVm.Gauge = 32;
            tempVm.Items = ti;

            var highP = _deviceType.ToItemValuesEnumerable(ItemFiles.PressureHighItems);
            var pi = _device.ItemGroup<PressureItems>(highP);
            var pressVm = new PressureFactorViewModel(pi, 80m, 14.73m);

            pressVm.Gauge = 100m;
            pressVm.AtmosphericGauge = 14.5m;

            var superVm = new SuperFactorViewModel(_device.ItemGroup<SuperFactorItems>(), tempVm, pressVm);

            var factor2 = tempVm.ExpectedValue;

            Assert.AreNotEqual(factor, factor2);
        }

        [TestMethod()]
        public async Task GetCurrentTestPointBuilderTest()
        {
            //var service = new VerificationViewModelService();
            //var vm = await service.CreateNewTest(_device);

            ////Assert.IsNotNull(vm);
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