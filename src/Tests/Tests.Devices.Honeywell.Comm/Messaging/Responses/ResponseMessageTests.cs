using Devices.Communications;
using Devices.Communications.IO;
using Devices.Honeywell.Comm;
using Devices.Honeywell.Comm.CommClients;
using Devices.Honeywell.Comm.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Tests.Devices.Honeywell.Comm.Clients;

namespace Tests.Devices.Honeywell.Comm.Messaging.Responses
{
    [TestClass]
    public class ResponseMessageTests : BaseHoneywellTest
    {
        [TestMethod]
        public async Task FramingError()
        {
            await Assert.ThrowsExceptionAsync<FailedConnectionException>(async () => await TryConnection(ResponseCode.FramingError));
        }

        [TestMethod]
        [ExpectedException(typeof(SignOnErrorException))]
        public async Task SignOnErrorThrown()
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
                .Callback(() => incoming.OnNext(Messages.Incoming.GetResponse(response)));

            return CommClient.CreateAsync(Device, commMock.Object);
        }
    }
}