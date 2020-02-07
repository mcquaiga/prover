using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Devices.Communications;
using Devices.Communications.Interfaces;
using Devices.Communications.IO;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Honeywell.Comm;
using Devices.Honeywell.Comm.CommClients;
using Devices.Honeywell.Comm.Exceptions;
using Devices.Honeywell.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tests.Honeywell.Comm.Messaging;

namespace Tests.Honeywell.Comm.Clients
{
    [TestClass]
    public class HoneywellClientTests : BaseHoneywellTest
    {
        private Mock<ICommPort> _commMock;
        private Subject<string> _incoming;
        private Subject<string> _outgoing;

        [TestInitialize]
        public void Initialize()
        {
            _commMock = new Mock<ICommPort>();
            _incoming = new Subject<string>();
            _outgoing = new Subject<string>();
            SetupComm(_commMock, _incoming, _outgoing);
        }

        private async Task<ICommunicationsClient<DeviceType, DeviceInstance>> GetConnectionAsync(ISubject<string> status = null)
        {
            MockConnectionHandshake(_commMock, _incoming);

            return await Device.ConnectAsync(_commMock.Object, statusObserver: status);
        }


        [TestMethod]
        public async Task ConnectionRetriedMultipleTimesTest()
        {
            var commMock = new Mock<ICommPort>();
            var incoming = new Subject<string>();
            var outgoing = new Subject<string>();

            SetupComm(commMock, incoming, outgoing);

            try
            {
                var conn = await Device.ConnectAsync(commMock.Object, 1, TimeSpan.FromMilliseconds(25));
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(TimeoutException));
            }

            await Assert.ThrowsExceptionAsync<FailedConnectionException>(async ()
                => await Device.ConnectAsync(commMock.Object, 0, TimeSpan.FromMilliseconds(25)));
        }

        [TestMethod]
        public async Task ConnectionSuccessTest()
        {
            var client = await GetConnectionAsync();

            Assert.IsTrue(client.IsConnected);
        }

        [TestMethod]
        public async Task ConnectionBadSignOnMessageFailureTest()
        {
            MockConnectionHandshake(_commMock, _incoming);

            _commMock.Setup(c => c.Send(Messages.Outgoing.SignOnMessage(Device)))
                .Callback(() => _incoming.OnNext(Messages.Incoming.GetResponse(ResponseCode.SignOnError)));
            
            await Assert.ThrowsExceptionAsync<SignOnErrorException>(async ()
                => await DeviceConnection.ConnectAsync(Device, _commMock.Object, 0, TimeSpan.FromMilliseconds(25)));
        }

        [TestMethod]
        public async Task CheckStatusLogsAreBeingWritten()
        {
            var status = new Subject<string>();
            var msg = String.Empty;
            status.Subscribe(s => msg = msg + s);
            
            var client = await GetConnectionAsync(status);
            Assert.IsFalse(msg.IsNullOrEmpty());
        }

        [TestMethod]
        public async Task ConnectionThrowsTimeoutException()
        {
            var commMock = new Mock<ICommPort>();
            var incoming = new Subject<string>();
            var outgoing = new Subject<string>();

            SetupComm(commMock, incoming, outgoing);

            try
            {
                var conn = await Device.ConnectAsync(commMock.Object, 0, TimeSpan.FromMilliseconds(50));
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