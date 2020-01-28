using System.Threading.Tasks;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Items.ItemGroups;
using Devices.Honeywell.Core.Repository;
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

            var instance = mini.CreateDeviceInstance(MiniMaxItemFile);

            Assert.IsNotNull(instance.Pressure);
        }

        [TestMethod]
        public async Task Test_GetPressureItemsFromJsonTest()
        {
            var mini = await _repo.GetByName("Mini-Max");

            var instance = mini.CreateDeviceInstance(MiniMaxItemFile);

            var myItems = MiniMaxPressureItemFile.ToItemValues(mini);
            var pItems = ItemGroupHelpers.GetItemGroup<IPressureItems>(myItems);

            Assert.IsNotNull(pItems);
        }

        [TestInitialize]
        public async Task Initialize()
        {
            _repo = new DeviceRepository(HoneywellDeviceDataSourceFactory.Instance);
            var all = await _repo.GetAll();
        }
    }
}