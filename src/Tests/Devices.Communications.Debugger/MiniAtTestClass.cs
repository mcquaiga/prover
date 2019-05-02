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

namespace Devices.Communications.Debugger
{
    [TestClass]
    public class MiniAtTestClass
    {
        private HoneywellClient _client;
        private SerialPort _comm;
        private IHoneywellDeviceType _miniAt;
        private IObserver<string> _receiveStream = new Subject<string>();

        [TestMethod]
        public async Task ConnectToMiniAt()
        {
            await _client.Connect(new System.Threading.CancellationToken(), 3, TimeSpan.FromSeconds(5));

            Assert.IsTrue(_client.IsConnected);
        }

        [TestInitialize]
        public async Task Init()
        {
            var repo = new DeviceRepository(DeviceDataSourceFactory.Instance);

            _miniAt = await repo.Find<IHoneywellDeviceType>(d => d.Name.Equals("Mini-AT"));

            _comm = new SerialPort("COM5", 9600);
            _client = new HoneywellClient(_comm, _miniAt);
            _client.Status.Subscribe(Console.WriteLine);
        }
    }
}