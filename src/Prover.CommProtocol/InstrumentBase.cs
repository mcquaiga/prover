using Akka.Actor;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.CommProtocol
{
    internal abstract class InstrumentBase : ReceiveActor
    {
        protected InstrumentType _instrumentType;
        protected ICommPort _commPort;

        public InstrumentBase(InstrumentType instrumentType, ICommPort commPort)
        {
            _instrumentType = instrumentType;

            _commPort = commPort;
            _commPort.DataReceivedObservable.Subscribe(args => DataReceived(args.Sender, args.EventArgs));
        }

        protected bool Connect()
        {
            return true;
        }

        protected void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

        }
    }
}
