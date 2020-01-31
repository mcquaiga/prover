using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Devices.Honeywell.Core;
using Devices.Honeywell.Core.Repository;
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
        public void Initialize()
        {
            var dataSourceMock = new Mock<IDeviceTypeDataSource<IHoneywellDeviceType>>();
            dataSourceMock.Setup(ds => ds.GetDeviceTypes())
                .Returns((IObservable<HoneywellDeviceType>) DevicesList.ToObservable());

            var sources = new List<IDeviceTypeDataSource<IHoneywellDeviceType>>()
            {
                MiJsonDeviceTypeDataSource.Instance,
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
            Assert.IsTrue(devices.Count() > 1);
            Assert.IsTrue(devices.First().Items.First(i => i.Number == 90).ItemDescriptions.Count > 0);
        }

        #endregion
    }
}