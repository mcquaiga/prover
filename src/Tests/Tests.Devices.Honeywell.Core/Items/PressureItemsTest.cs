using System.Threading.Tasks;
using Devices.Core.Interfaces.Items;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Devices.Honeywell.Core.Items
{
    [TestClass]
    public class PressureItemsTest : BaseHoneywellTest
    {
        [TestMethod]
        public async Task Create()
        {
            var miniAt = await _repo.GetByName("Mini-AT");
        }

        [TestMethod]
        public async Task GetPressureItemsFromDeviceType()
        {
            var mini = await _repo.GetByName("Mini-Max");

            var itemFile = mini.Convert(MiniMaxItemFile);
            var instance = mini.CreateInstance(itemFile);

            var press = instance.GetItemValuesByGroup<IPressureItems>();

            Assert.IsNotNull(press);
        }

        [TestMethod]
        public async Task GetPressureItemsFromJsonTest()
        {
            var mini = await _repo.GetByName("Mini-Max");

            var itemFile = mini.Convert(MiniMaxItemFile);
            var instance = mini.CreateInstance(itemFile);

            var myItems = instance.Convert(MiniMaxPressureItemFile);
            var pitems = instance.GetItemValuesByGroup<IPressureItems>(myItems);

            Assert.IsNotNull(pitems);
        }

        [TestInitialize]
        public async Task Start()
        {
            _repo = new DeviceRepository(HoneywellDeviceDataSourceFactory.Instance);
            var all = await _repo.GetAll();
        }

        private DeviceRepository _repo;
    }
}