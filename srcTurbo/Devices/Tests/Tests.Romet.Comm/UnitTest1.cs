using System;
using System.Threading.Tasks;
using Devices.Communications;
using Devices.Communications.IO;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
using Devices.Romet.Comm;
using Devices.Romet.Core;
using Devices.Romet.Core.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Romet.Comm
{
    [TestClass]
    public class UnitTest1
    {
        private RometDeviceType _adem;
        public DeviceRepository Repository { get; private set; }

        [TestInitialize]
        public async Task Initialize()
        {
            Repository = await DeviceRepository.Instance.RegisterDataSourceAsync(RometDeviceRepository.DataSource);
            _adem = (RometDeviceType) Repository.GetByName("Adem");
        }

        [TestMethod]
        public async Task TestingSerialCommunications()
        {
            var commPort = new SerialPort("COM1", 9600);
            var client = await RometClientFactory.CreateAsync(_adem, commPort);

            await client.ConnectAsync();
            var items = await client.GetItemsAsync();
            await client.Disconnect();

            var instance = _adem.CreateInstance(items);

            var site = instance.ItemGroup<ISiteInformationItems>();
            var pressure = instance.ItemGroup<IPressureItems>();
            var temp = instance.ItemGroup<ITemperatureItems>();
            var v = instance.ItemGroup<IVolumeItems>();

            Assert.IsNotNull(site);
            Assert.IsNotNull(pressure);
            Assert.IsNotNull(temp);
        }
    }
}
