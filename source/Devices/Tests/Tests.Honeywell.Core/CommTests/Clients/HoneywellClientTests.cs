using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Devices.Communications;
using Devices.Communications.Interfaces;
using Devices.Communications.IO;
using Devices.Communications.Status;
using Devices.Honeywell.Comm;
using Devices.Honeywell.Comm.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prover.Shared.IO;
using Messages = Tests.Honeywell.Comm.Messaging.Messages;

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

            var commFactoryMock = new Mock<ICommPortFactory>();
            commFactoryMock.Setup(c => c.Create(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(_commMock.Object);

            ClientFactory = new CommunicationsClientFactory(commFactoryMock.Object);

        }

        private async Task<ICommunicationsClient> GetConnectionAsync(ISubject<string> status = null)
        {
            MockConnectionHandshake(_commMock, _incoming);

            return ClientFactory.Create(Device, _commMock.Object);
        }


        [TestMethod]
        public async Task ConnectionRetriedMultipleTimesTest()
        {
            var commMock = new Mock<ICommPort>();
            var incoming = new Subject<string>();
            var outgoing = new Subject<string>();

            SetupComm(commMock, incoming, outgoing);
            var client = ClientFactory.Create(Device, _commMock.Object);
            try
            {

                var conn = client.ConnectAsync(0, TimeSpan.FromMilliseconds(25));
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex.InnerException, typeof(TimeoutException));
            }

            await Assert.ThrowsExceptionAsync<TimeoutException>(async ()
                => await client.ConnectAsync(0, TimeSpan.FromMilliseconds(25)));
        }

        [TestMethod]
        public async Task ConnectionSuccessTest()
        {
            var client = await GetConnectionAsync();
            await client.ConnectAsync();
            Assert.IsTrue(client.IsConnected);
        }

        [TestMethod]
        public async Task ConnectionBadSignOnMessageFailureTest()
        {
            MockConnectionHandshake(_commMock, _incoming);
            var client = ClientFactory.Create(Device, _commMock.Object);
            _commMock.Setup(c => c.Send(Messages.Outgoing.SignOnMessage(Device)))
                .Callback(() => _incoming.OnNext(Messages.Incoming.GetResponse(ResponseCode.SignOnError)));
            
            await Assert.ThrowsExceptionAsync<SignOnErrorException>(async ()
                => await client.ConnectAsync(0, TimeSpan.FromMilliseconds(25)));
        }

        [TestMethod]
        public async Task CheckStatusLogsAreBeingWritten()
        {
            MockConnectionHandshake(_commMock, _incoming);

            var status = new Subject<StatusMessage>();
            var msg = String.Empty;
            status.Subscribe(s => msg = msg + s.ToString());
            
            var client = ClientFactory.Create(Device, _commMock.Object);
            client.StatusMessageObservable.Subscribe(status);

            await client.ConnectAsync();
            Assert.IsFalse(msg.IsNullOrEmpty());
        }

        [TestMethod]
        public async Task ConnectionThrowsTimeoutException()
        {
            var commMock = new Mock<ICommPort>();
            var incoming = new Subject<string>();
            var outgoing = new Subject<string>();

            SetupComm(commMock, incoming, outgoing);
            var client = ClientFactory.Create(Device, _commMock.Object);

            await Assert.ThrowsExceptionAsync<TimeoutException>(async ()
                => await client.ConnectAsync(0, TimeSpan.FromMilliseconds(50)));
        }
    }
}