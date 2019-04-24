using Devices.Communications;
using Devices.Communications.IO;
using Devices.Honeywell.Comm;
using Devices.Honeywell.Comm.CommClients;
using Devices.Honeywell.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.Devices.Honeywell.Comm
{
    public abstract class BaseHoneywellClientTests
    {
        #region Methods

        [TestInitialize]
        public virtual void Setup()
        {
            SetupDevice();
        }

        public virtual void SetupDevice()
        {
            Device = new Mock<IHoneywellEvcType>();

            Device.Setup(d => d.Name)
                .Returns("Honeywell");

            Device.Setup(d => d.Id)
                .Returns(3);

            Device.Setup(d => d.AccessCode)
                .Returns(3);
        }

        #endregion

        #region Fields

        protected Mock<IHoneywellEvcType> Device;

        #endregion

        protected void MockConnectionHandshake(Mock<ICommPort> commMock, ISubject<string> incomingStream)
        {
            commMock.Setup(c => c.Send(It.IsAny<string>()))
              .Returns(Task.FromResult(default(object)));

            commMock.Setup(c => c.Send(ControlCharacters.ENQ.ToString()))
               .Callback(() => incomingStream.OnNext(ControlCharacters.ACK.ToString()));

            commMock.Setup(c => c.Send(Messages.Outgoing.SignOnMessage))
                .Callback(() => incomingStream.OnNext(Messages.Incoming.SuccessResponse));
        }

        protected void SetupComm(Mock<ICommPort> commMock, IObservable<string> readStream, ISubject<string> writeStream)
        {
            var dataStream = new Subject<char>();
            readStream.SelectMany(s => s.ToObservable())
                .Subscribe(dataStream);

            commMock.Setup(c => c.Name)
                .Returns("MOCK1");

            commMock.Setup(c => c.IsOpen())
                .Returns(true);

            var connectable = dataStream.Publish();
            commMock.Setup(c => c.DataReceivedObservable)
                .Returns(connectable);
            connectable.Connect();

            commMock.Setup(c => c.DataSentObservable)
                .Returns(writeStream);
        }
    }

    [TestClass]
    public class HoneywellClientTests : BaseHoneywellClientTests
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

            client.Status
                .Subscribe(s => Assert.IsNotNull(s));

            await client.Connect(new CancellationToken(), retryAttempts: 0);

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
                await client.Connect(new CancellationToken(), retryAttempts: 0, TimeSpan.FromMilliseconds(100));
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

    [TestClass]
    public class ResponseMessageTests : BaseHoneywellClientTests
    {
        #region Methods

        [TestMethod]
        public async Task FramingError()
        {
            await TryConnection(ResponseCode.FramingError);
        }

        [TestMethod]
        [ExpectedException(typeof(SignOnErrorException))]
        public async Task ThrowsSignOnError()
        {
            await TryConnection(ResponseCode.SignOnError);
        }

        private Task TryConnection(ResponseCode response)
        {
            var commMock = new Mock<ICommPort>();
            var incoming = new Subject<string>();
            var outgoing = new Subject<string>();

            SetupComm(commMock, incoming, outgoing);

            commMock.Setup(c => c.Send(It.IsAny<string>()))
             .Returns(Task.FromResult(default(object)));

            commMock.Setup(c => c.Send(ControlCharacters.ENQ.ToString()))
               .Callback(() => incoming.OnNext(ControlCharacters.ACK.ToString()));

            commMock.Setup(c => c.Send(It.Is<string>(s => s.Length > 1)))
                .Callback(() => incoming.OnNext(Messages.Incoming.GetResponse(response)))
                ;

            var client = new HoneywellClient(commMock.Object, Device.Object);

            return client.Connect(new CancellationToken(), retryAttempts: 2, TimeSpan.FromMilliseconds(100));
        }

        #endregion
    }
}