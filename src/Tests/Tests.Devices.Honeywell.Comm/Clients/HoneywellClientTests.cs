using Devices.Communications;
using Devices.Communications.IO;
using Devices.Honeywell.Comm.CommClients;
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
        #region Methods

        [TestMethod]
        public async Task ConnectionSuccessTest()
        {
            var commMock = new Mock<ICommPort>();
            var incoming = new Subject<string>();
            var outgoing = new Subject<string>();

            SetupComm(commMock, incoming, outgoing);
            MockConnectionHandshake(commMock, incoming);

            var client = new HoneywellClient(commMock.Object, Device.Object);

            client.Status.Subscribe(s => Assert.IsNotNull(s));

            await client.Connect(new CancellationToken(), retryAttempts: 0, TimeSpan.FromMilliseconds(500));

            Assert.IsTrue(client.IsConnected);
        }

        [TestMethod]
        public async Task ConnectionTimedoutTest()
        {
            var commMock = new Mock<ICommPort>();
            var incoming = new Subject<string>();
            var outgoing = new Subject<string>();

            SetupComm(commMock, incoming, outgoing);

            var client = new HoneywellClient(commMock.Object, Device.Object);

            try
            {
                await client.Connect(new CancellationToken(), retryAttempts: 1, TimeSpan.FromMilliseconds(100));
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(TimeoutException));
            }

            await Assert.ThrowsExceptionAsync<FailedConnectionException>(()
                => client.Connect(new CancellationToken(), retryAttempts: 0, TimeSpan.FromMilliseconds(100)));
        }

        #endregion
    }
}