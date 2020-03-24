using System;
using System.Threading.Tasks;
using Devices.Communications.IrDa;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace Devices.Communications.Debugger
{
    [TestClass]
    [TestCategory("CommPorts")]
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