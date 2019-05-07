using System;
using System.Threading.Tasks;
using Devices.Communications.IrDa;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace Tests.Devices.Honeywell.Comm
{
    [TestClass]
    public class CommPortTests
    {
        [TestMethod]
        public async Task DiscoverIrDADevicesTest()
        {
            //var ir = new IrDAPort();
            //await ir.Open(new CancellationToken());

            var devices = await IrDAPort.GetIrDADevices();
            Assert.IsNotNull(devices);
        }
    }
}