using Akka.Actor;
using Prover.CommProtocol.IO;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prover.CommProtocol
{ 
    public class CommunicationClient
    {
        protected IActorRef _port;
        protected IActorRef _connection;
        public InstrumentType InstrumentType { get; private set; }

        private CommunicationClient(InstrumentType instrumentType, CommPort commPort, ActorSystem actorSystem)
        {
            InstrumentType = instrumentType;

            _port = actorSystem.ActorOf(Props.Create(() => new PortCoordinator(commPort)));
            _connection = actorSystem.ActorOf(Props.Create(() => new Connection(_port, InstrumentType.MiniMax)));
        }

        public void Connect()
        {
            _connection.Tell(new TryConnectCommand());
        }

        public void Disconnect()
        {
            _connection.Tell(new TryDisconnectCommand());
        }

        public static CommunicationClient Create(InstrumentType instrumentType, ActorSystem actorSystem = null)
        {
            if (actorSystem == null)
                actorSystem = ActorSystem.Create("ProtocolActorSystem");

            var port = new SerialCommPort("COM3", 9600, 500);

            return new CommunicationClient(instrumentType, port, actorSystem);
        }
    }
    
}
