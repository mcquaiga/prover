using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Devices.Communications.IO;
using Devices.Honeywell.Core;
using Moq;
using Tests.Honeywell.Comm.Messaging;

namespace Tests.Honeywell.Comm
{
    public static class CommTestHelpers
    {
        internal static void MockConnectionHandshake(Mock<ICommPort> commMock, Mock<HoneywellDeviceType> deviceType, ISubject<string> incomingStream)
        {
            commMock.Setup(c => c.Send(It.IsAny<string>()))
              .Returns(Task.FromResult(default(object)));

            commMock.Setup(c => c.Send(ControlCharacters.ENQ.ToString()))
               .Callback(() => incomingStream.OnNext(ControlCharacters.ACK.ToString()));

            commMock.Setup(c => c.Send(Messages.Outgoing.SignOnMessage(deviceType.Object)))
                .Callback(() => incomingStream.OnNext(Messages.Incoming.SuccessResponse));
        }

        internal static void SetupComm(Mock<ICommPort> commMock, IObservable<string> readStream, ISubject<string> writeStream)
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
}