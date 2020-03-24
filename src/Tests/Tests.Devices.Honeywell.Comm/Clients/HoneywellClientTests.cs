using Devices.Communications;
using Devices.Communications.IO;
using Devices.Honeywell.Comm.CommClients;
using Devices.Honeywell.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.Devices.Honeywell.Comm.Clients
{
    [TestClass]
    public class HoneywellClientTests : BaseHoneywellTest
    {
        [TestMethod]
        public async Task ConnectionRetriedMultipleTimesTest()
        {
            var commMock = new Mock<ICommPort>();
            var incoming = new Subject<string>();
            var outgoing = new Subject<string>();

            SetupComm(commMock, incoming, outgoing);

            try
            {
                var conn = await DeviceConnection.ConnectAsync(Device, commMock.Object, 1, TimeSpan.FromMilliseconds(50));
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(TimeoutException));
            }

            await Assert.ThrowsExceptionAsync<FailedConnectionException>(async ()
                => await DeviceConnection.ConnectAsync(Device, commMock.Object, 0, TimeSpan.FromMilliseconds(50)));
        }

        [TestMethod]
        public async Task ConnectionSuccessTest()
        {
            var commMock = new Mock<ICommPort>();
            var incoming = new Subject<string>();
            var outgoing = new Subject<string>();

            SetupComm(commMock, incoming, outgoing);
            MockConnectionHandshake(commMock, incoming);

            var client = await DeviceConnection.ConnectAsync(Device, commMock.Object);

            client.Status.Subscribe(s => Assert.IsNotNull(s));

            Assert.IsTrue(client.IsConnected);
        }

        [TestMethod]
        public async Task ConnectionTimedoutTest()
        {
            var commMock = new Mock<ICommPort>();
            var incoming = new Subject<string>();
            var outgoing = new Subject<string>();

            SetupComm(commMock, incoming, outgoing);

            try
            {
                var conn = await DeviceConnection.ConnectAsync(Device, commMock.Object, 0, TimeSpan.FromMilliseconds(50));
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(TimeoutException));
            }

            await Assert.ThrowsExceptionAsync<FailedConnectionException>(async ()
                => await DeviceConnection.ConnectAsync(Device, commMock.Object, 0, TimeSpan.FromMilliseconds(50)));
        }
    }
}