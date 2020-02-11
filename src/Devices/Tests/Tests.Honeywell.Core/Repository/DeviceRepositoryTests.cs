using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Devices.Core.Repository;
using Devices.Honeywell.Core;
using Devices.Honeywell.Core.Repository.JsonRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Tests.Honeywell.Core.Repository
{
    [TestClass]
    public class JsonDeviceRepositoryTests : BaseHoneywellTest
    {
        #region Properties

        public DeviceRepository Repository { get; private set; }

        #endregion

        #region Methods

        [TestInitialize]
        public async Task Initialize()
        {
            var dataSourceMock = new Mock<IDeviceTypeDataSource<HoneywellDeviceType>>();
            dataSourceMock.Setup(ds => ds.GetDeviceTypes())
                .Returns(DevicesList.ToObservable());

            var sources = new List<IDeviceTypeDataSource<HoneywellDeviceType>>()
            {
                MiJsonDeviceTypeDataSource.Instance,
                dataSourceMock.Object
            };

            Repository = DeviceRepository.Instance.RegisterDataSource(sources);
        }

        #endregion

        #region Methods

        [TestMethod]
        public async Task GetDevicesFromCoreRepository()
        {
            var devices = Repository.GetAll();

            Assert.IsNotNull(devices);
            Assert.IsTrue(devices.Count() > 1);
            Assert.IsTrue(devices.First().Items.First(i => i.Number == 90).ItemDescriptions.Count > 0);
        }

        #endregion
    }
}