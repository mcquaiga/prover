using System;
using System.Threading.Tasks;
using Devices;
using Devices.Core.Items;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Repository.JsonRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Honeywell.Core.Items
{
    [TestClass]
    public class PressureItemsTest : BaseHoneywellTest
    {
        private IDeviceRepository _repo;

        [TestInitialize]
        public async Task Initialize()
        {
            _repo = new DeviceRepository();
            await _repo.UpdateCachedTypes(MiJsonDeviceTypeDataSource.Instance);
        }

        [TestMethod]
        public void Test_GetMiniMaxInstance()
        {
            var device = _repo.GetByName("Mini-Max");
            var instance = device.CreateInstance(MiniMaxItemFile);
            var pItems = device.GetItemMetadata<PressureItems>();
            Assert.IsNotNull(instance.Values);
            Assert.IsFalse(instance.Values.Count == 0);
        }

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
            var pItems = device.GetGroupValues<PressureItems>(myItems);
            var pItems2 = instance.ItemGroup<PressureItems>();

            var t = instance.ItemGroup<TemperatureItems>();
            var v = instance.ItemGroup<VolumeItems>();
            var e = instance.ItemGroup<EnergyItems>();
            var s = instance.ItemGroup<SiteInformationItems>();
            var rotary = instance.ItemGroup<RotaryMeterItems>();
            var super = instance.ItemGroup<SuperFactorItems>();

            Assert.IsNotNull(pItems);
            Assert.IsNotNull(instance.Values);
            Assert.IsFalse(instance.Values.Count == 0);
        }
    }
}