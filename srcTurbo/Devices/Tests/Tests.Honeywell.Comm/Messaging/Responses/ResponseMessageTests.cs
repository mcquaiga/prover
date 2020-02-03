using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Devices.Communications;
using Devices.Communications.Interfaces;
using Devices.Communications.IO;
using Devices.Core.Interfaces;
using Devices.Honeywell.Comm;
using Devices.Honeywell.Comm.CommClients;
using Devices.Honeywell.Comm.Exceptions;
using Devices.Honeywell.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Tests.Honeywell.Comm.Messaging.Responses
{
    [TestClass]
    public class ResponseMessageTests : BaseHoneywellTest
    {
        [TestMethod]
        [ExpectedException(typeof(HoneywellResponseException))]
        public async Task FramingError()
        {
            await TryConnection(ResponseCode.FramingError);
        }

        [TestMethod]
        [ExpectedException(typeof(SignOnErrorException))]
        public async Task SignOnErrorThrown()
        {
            await TryConnection(ResponseCode.SignOnError);
        }

        private Task<ICommunicationsClient<DeviceType, DeviceInstance>> TryConnection(ResponseCode response)
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

            return Device.ConnectAsync(commMock.Object, 0, TimeSpan.FromMilliseconds(50));
        }
    }
}