using System;
using System.Threading.Tasks;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Items;
using Devices.Honeywell.Core.Repository.JsonRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Honeywell.Core.Items
{
    [TestClass]
    public class PressureItemsTest : BaseHoneywellTest
    {
        private DeviceRepository _repo;

        [TestMethod]
        public void Test_GetPressureItemsFromDeviceType()
        {
            var mini = _repo.GetByName("Mini-Max");

            var instance = mini.CreateInstance(MiniMaxItemFile);

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void Test_GetPressureItemsFromJsonTest()
        {
            var device = _repo.GetByName("Mini-Max");
            var instance = device.CreateInstance(MiniMaxItemFile);

            var myItems = device.ToItemValuesEnumerable(MiniMaxPressureItemFile);
            var pItems = device.GetGroupValues<IPressureItems>(myItems);
            var pItems2 = instance.ItemGroup<IPressureItems>();

            var t = instance.ItemGroup<ITemperatureItems>();
            var v = instance.ItemGroup<IVolumeItems>();
            var e = instance.ItemGroup<IEnergyItems>();
            var s = instance.ItemGroup<ISiteInformationItems>();
            var rotary = instance.ItemGroup<IRotaryMeterItems>();
            var super = instance.ItemGroup<ISuperFactorItems>();

            Assert.IsNotNull(pItems);
            Assert.IsNotNull(instance.Values);
            Assert.IsFalse(instance.Values.Count == 0);
        }

        [TestMethod]
        public void Test_GetMiniMaxInstance()
        {
            var device = _repo.GetByName("Mini-Max");
            var instance = device.CreateInstance(MiniMaxItemFile);
            var pItems = device.GetItemMetadata<IPressureItems>();
            Assert.IsNotNull(instance.Values);
            Assert.IsFalse(instance.Values.Count == 0);
           
            Console.WriteLine("");
        }

        [TestInitialize]
        public async Task Initialize()
        {
            _repo = await DeviceRepository.Instance.RegisterDataSourceAsync(MiJsonDeviceTypeDataSource.Instance);
        }
    }
}