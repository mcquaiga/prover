using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
using Devices.Honeywell.Core;
using Devices.Honeywell.Core.Items;
using Devices.Honeywell.Core.Items.ItemGroups;
using Devices.Honeywell.Core.Repository;
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
            var pItems = instance.GetItemsByGroup<IPressureItems>(myItems);

            Assert.IsNotNull(pItems);
            Assert.IsNotNull(instance.ItemValues);
            Assert.IsFalse(instance.ItemValues.Count == 0);
        }

        [TestMethod]
        public void Test_GetMiniMaxInstance()
        {
            var device = _repo.GetByName("Mini-Max");
            var instance = device.CreateInstance(MiniMaxItemFile);
            var pItems = device.GetItemsByGroup<IPressureItems>();
            Assert.IsNotNull(instance.ItemValues);
            Assert.IsFalse(instance.ItemValues.Count == 0);
           
            Console.WriteLine("");
        }

        [TestInitialize]
        public async Task Initialize()
        {
            _repo = await DeviceRepository.Instance.RegisterDataSourceAsync(MiJsonDeviceTypeDataSource.Instance);
        }
    }
}