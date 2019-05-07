using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Devices.Communications.IO;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Devices.Honeywell.Comm.CommClients;
using Devices.Honeywell.Core;
using Devices.Honeywell.Core.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using Devices.Communications.Interfaces;
using Devices.Core.Interfaces.Items;
using ExpectedObjects;

namespace Devices.Communications.Debugger
{
    [TestClass]
    public class MiniAtTestClass
    {
        [TestCleanup]
        public async Task Cleanup()
        {
            await _client.Disconnect();
            _comm.Dispose();
        }

        [TestMethod]
        public async Task ConnectToMiniAt()
        {
            await CreateConnectionAsync();
            Assert.IsTrue(_client.IsConnected);
        }

        [TestMethod]
        public async Task CreateDevice()
        {
            await CreateConnectionAsync();

            var device = await _client.GetDeviceAsync();

            var p1 = device.GetItemValuesByGroup<IPressureItems>();
            var p2 = await _client.GetItemsAsync<IPressureItems>();

            await _client.Disconnect();

            Assert.IsNotNull(device);

            p1.ToExpectedObject().ShouldEqual(p2);
        }

        [TestMethod]
        public async Task GetSiteInformationItems()
        {
            await CreateConnectionAsync();

            var device = await _client.GetDeviceAsync();
            var siteInfo = await _client.GetItemsAsync<ISiteInformationItems>();

            Assert.IsTrue(_client.IsConnected);
            Assert.IsNotNull(siteInfo);
        }

        [TestInitialize]
        public async Task InitAsync()
        {
            var repo = new DeviceRepository(HoneywellDeviceDataSourceFactory.Instance);
            _miniAt = await repo.Find<HoneywellDeviceType>(d => d.Name.Equals("Mini-AT"));
            _comm = new SerialPort("COM6", 9600);
        }

        private ICommunicationsClient _client;
        private SerialPort _comm;
        private HoneywellDeviceType _miniAt;

        private async Task CreateConnectionAsync()
        {
            await Task.Delay(500);
            _client = await DeviceConnection.ConnectAsync(_miniAt, _comm);
            _client.Status.Subscribe(Console.WriteLine);
        }
    }
}