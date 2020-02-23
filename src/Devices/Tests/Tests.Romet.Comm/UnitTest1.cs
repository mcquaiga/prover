using System;
using System.Threading.Tasks;
using Devices.Communications.Interfaces;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
using Devices.Romet.Core;
using Devices.Romet.Core.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Console = System.Console;

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
            Repository = new DeviceRepository();
            await Repository.UpdateCachedTypes(RometJsonDeviceTypeDataSource.Instance);
            _adem = (RometDeviceType) Repository.GetByName("Adem");
        }

        [TestMethod]
        public async Task TestingSerialCommunications()
        {
            //var commPort = new SerialPort("COM1", 9600);
            var client = new CommunicationsClientFactory(new CommPortFactory()).Create(_adem, "COM1", 9600);


            client.StatusMessageObservable.Subscribe(Console.WriteLine);

            await client.ConnectAsync();
            var items = await client.GetItemsAsync();
            await client.Disconnect();

            var instance = _adem.Factory.CreateInstance(items);
            var rt = instance.ItemGroup<RotaryMeterItems>();

            var site = instance.ItemGroup<SiteInformationItems>();
            var pressure = instance.ItemGroup<PressureItems>();
            var temp = instance.ItemGroup<TemperatureItems>();
            var v = instance.ItemGroup<VolumeItems>();
            VolumeItems vc = instance.ItemGroup<VolumeItems>();

            Assert.IsNotNull(site);
            Assert.IsNull(pressure);
            Assert.IsNotNull(temp);
        }
    }
}