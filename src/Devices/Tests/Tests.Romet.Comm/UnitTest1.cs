using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Devices.Communications.IO;
using Devices.Core.Items;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
using Devices.Romet.Comm;
using Devices.Romet.Core;
using Devices.Romet.Core.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

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
            var client = RometClientFactory.Create(_adem, commPort);
            
            client.CommunicationMessages.Subscribe(Console.WriteLine);
            client.StatusMessages.Subscribe(Console.WriteLine);

            await client.ConnectAsync();
            var items = await client.GetItemsAsync();
            await client.Disconnect();

            var instance = _adem.Factory.CreateInstance(items);
            var rt = instance.ItemGroup<IRotaryMeterItems>();

            var site = instance.ItemGroup<ISiteInformationItems>();
            var pressure = instance.ItemGroup<IPressureItems>();
            var temp = instance.ItemGroup<ITemperatureItems>();
            var v = instance.ItemGroup<IVolumeItems>();
            IVolumeCorrectedItems vc = instance.ItemGroup<IVolumeCorrectedItems>();


         

            Assert.IsNotNull(site);
            Assert.IsNull(pressure);
            Assert.IsNotNull(temp);
        }
    }
}
