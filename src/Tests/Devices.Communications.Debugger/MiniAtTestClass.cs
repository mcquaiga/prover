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

namespace Devices.Communications.Debugger
{
    [TestClass]
    public class MiniAtTestClass
    {
        private HoneywellClient _client;
        private SerialPort _comm;
        private IHoneywellDeviceType _miniAt;

        [TestCleanup]
        public async Task CleanUpAsync()
        {
            await _client?.Disconnect();
            _comm.Dispose();
        }

        [TestMethod]
        public async Task ConnectToMiniAt()
        {
            Assert.IsTrue(_client.IsConnected);

            await _client.Disconnect();
        }

        [TestInitialize]
        public async Task InitAsync()
        {
            var repo = new DeviceRepository(DeviceDataSourceFactory.Instance);

            _miniAt = await repo.Find<IHoneywellDeviceType>(d => d.Name.Equals("Mini-AT"));

            _comm = new SerialPort("COM5", 9600);
            _client = (HoneywellClient)await CommClient.CreateAsync(_miniAt, _comm);
            _client.Status.Subscribe(Console.WriteLine);
        }
    }
}