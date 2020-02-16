using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Devices.Core.Repository;
using Devices.Romet.Core;
using Devices.Romet.Core.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Romet.Core
{
    [TestClass]
    public class RometJsonDeviceRepositoryTests
    {
        #region Properties

        public DeviceRepository Repository { get; private set; }

        #endregion

        #region Methods

        [TestInitialize]
        public async Task Initialize()
        {
            //var dataSourceMock = new Mock<IDeviceTypeDataSource<IRometDeviceType>>();
            //dataSourceMock.Setup(ds => ds.GetDeviceTypes())
            //    .Returns((IObservable<IRometDeviceType>) DevicesList.ToObservable());

            //var sources = new List<IDeviceTypeDataSource<IHoneywellDeviceType>>()
            //{
            //    JsonDeviceTypeDataSource.Instance,
            //    dataSourceMock.Object
            //};

           // Repository = await new DeviceRepository().DeviceDataSource.RegisterDataSourceAsync(RometDeviceRepository.DataSource);
        }


        #endregion

        #region Methods

        [TestMethod]
        public async Task LoadDeviceSuccessfulTest()
        {
            var ds = RometJsonDeviceTypeDataSource.Instance;

            var devices = new ConcurrentBag<RometDeviceType>();

           await ds.GetDeviceTypes()
               .ForEachAsync(d =>
                {
                    Assert.IsNotNull(d);
                    Assert.IsNotNull(d.CanUseIrDaPort);
                    Assert.IsTrue(d.Id != Guid.Empty);

                    Assert.IsFalse(d.Items
                        .GroupBy(n => n.Number)
                        .Any(c => c.Count() > 1));

                    devices.Add(d as RometDeviceType);
                });

            Assert.IsTrue(devices.Count == 1);
        }

        [TestMethod]
        public async Task GetDevicesFromCoreRepository()
        {
            var devices = Repository.GetAll();

            Assert.IsNotNull(devices);
            Assert.IsTrue(devices.Count() == 1);
            Assert.IsTrue(devices.First().Items.First(i => i.Number == 90).ItemDescriptions.Count > 0);
        }

        #endregion
    }
}