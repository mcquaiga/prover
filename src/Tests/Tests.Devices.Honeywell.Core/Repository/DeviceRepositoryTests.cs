using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Tests.Devices.Honeywell.Core
{
    [TestClass]
    public class DeviceRepositoryTests : BaseHoneywellTest
    {
        #region Properties

        public DeviceRepository Repository { get; private set; }

        #endregion

        #region Methods

        [TestInitialize]
        public void Initialize()
        {
            var dataSourceMock = new Mock<IDeviceDataSource<IDeviceType>>();
            dataSourceMock.Setup(ds => ds.GetDeviceTypes())
                .Returns(DevicesList.ToObservable());

            var sources = new List<IDeviceDataSource<IDeviceType>>()
            {
                DeviceDataSourceFactory.Instance,
                dataSourceMock.Object
            };

            Repository = new DeviceRepository(sources);
        }

        #endregion

        #region Methods

        [TestMethod]
        public async Task GetDevicesFromCoreRepository()
        {
            var devices = await Repository.GetAll();

            Assert.IsNotNull(devices);
            Assert.IsTrue(devices.Count() == 8);
        }

        #endregion
    }
}