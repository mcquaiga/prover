using System.Threading.Tasks;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
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
        public async Task Test_GetPressureItemsFromDeviceType()
        {
            var mini = await _repo.GetByName("Mini-Max");

            var instance = mini.InstanceFactory.CreateInstance(MiniMaxItemFile);

            Assert.IsNotNull(instance.Pressure);
        }

        [TestMethod]
        public async Task Test_GetPressureItemsFromJsonTest()
        {
            var mini = await _repo.GetByName("Mini-Max");

            var instance = mini.InstanceFactory.CreateInstance(MiniMaxItemFile);

            var myItems = mini.ConvertKeyValuesToItemValues(MiniMaxPressureItemFile);
            var pItems = instance.GetItemsByGroup<IPressureItems>(myItems);

            Assert.IsNotNull(pItems);
        }

        [TestInitialize]
        public async Task Initialize()
        {
            _repo = new DeviceRepository(MiJsonDeviceTypeDataSource.Instance);
            var all = await _repo.GetAll();
        }
    }
}