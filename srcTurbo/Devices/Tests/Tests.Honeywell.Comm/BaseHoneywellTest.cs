using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Devices.Communications.IO;
using Devices.Core.Items;
using Devices.Honeywell.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tests.Honeywell.Comm.Messaging;

namespace Tests.Honeywell.Comm
{
    public abstract class BaseHoneywellTest
    {
        protected HoneywellDeviceType Device;

        [TestInitialize]
        public virtual void Setup()
        {
            SetupDevice();
        }

        public virtual void SetupDevice()
        {
            Device = new HoneywellDeviceType(new List<ItemMetadata>
                {
                    new ItemMetadata() { Number= 0 },
                    new ItemMetadata() { Number= 2 }
                })
            {
                Name = "Mini-At",
                Id = 3,
                AccessCode = 3
            };
        }

        protected void MockConnectionHandshake(Mock<ICommPort> commMock, ISubject<string> incomingStream)
        {
            commMock.Setup(c => c.Send(It.IsAny<string>()))
              .Returns(Task.FromResult(default(object)));

            commMock.Setup(c => c.Send(ControlCharacters.ENQ.ToString()))
               .Callback(() => incomingStream.OnNext(ControlCharacters.ACK.ToString()));

            commMock.Setup(c => c.Send(Messages.Outgoing.SignOnMessage(Device)))
                .Callback(() => incomingStream.OnNext(Messages.Incoming.SuccessResponse));
        }

        protected void MockReadItem(Mock<ICommPort> commMock, ISubject<string> incomingStream)
        {
            //commMock.Setup(c => c.Send(It.Is<string>(s => s.Contains("RD"))))
            //    .Callback(() => incomingStream.OnNext(Messages.Incoming));
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
}