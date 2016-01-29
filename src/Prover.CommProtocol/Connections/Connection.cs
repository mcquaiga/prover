using Akka.Actor;
using NLog;
using Prover.CommProtocol.Communications;
using Prover.CommProtocol.IO;
using Prover.CommProtocol.Messages.Events;
using System;
using System.Threading.Tasks;

namespace Prover.CommProtocol
{
    public enum ConnectionState
    {
        Unlinked,
        Linked    
    }

    public enum HandshakeState
    {
        Idle,
        WakingItUp,
        SigningOn,
        SigningOff
    }

    public class Connection : ReceiveActor
    {
        private const int AccessCode = 33333;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        protected InstrumentType _instrumentType;
        protected IActorRef _portActor;

        public Connection(IActorRef portActor, InstrumentType instrumentType)
        {
            _portActor = portActor;
            _instrumentType = instrumentType;
            UnlinkedIdle();
        }

        private void UnlinkedIdle()
        {
            Receive<TryConnectCommand>(_ =>
            {
                Become(Busy);
                TryConnect().PipeTo(Self);
            });
        }

        private void Busy()
        {
            Receive<CancelRequested>(_ => { });

            Receive<ConnectionState>(state =>
            {
                if (state == ConnectionState.Linked)
                    Become(Linked);
                else
                {
                    Become(UnlinkedIdle);
                }
            });
        }

        private void Linked()
        {
            Receive<TryDisconnectCommand>(_ =>
            {
                Become(Busy);
                TryDisconnect().PipeTo(Self);
            });

            Receive<TryConnectCommand>(_ =>
            {
                Sender.Tell(CommState.LinkedIdle);
            });
        }

        protected async virtual Task<ConnectionState> TryConnect()
        {
            Context.System.EventStream.Publish(new ConnectingToInstrumentEvent("Serial"));

            return await WakeUpInstrument().ContinueWith(_ =>
            {
                SignOn().Wait();
                return ConnectionState.Linked;
            });           
        }

        protected async virtual Task<ConnectionState> TryDisconnect()
        {
            Context.System.EventStream.Publish(HandshakeState.SigningOff);

            return await _portActor.Ask(Commands.SignOffCommand())
                .ContinueWith(_ =>
                {
                    return ConnectionState.Unlinked;
                });
        }

        private async Task SignOn()
        {
            Context.System.EventStream.Publish(HandshakeState.SigningOn);

            await _portActor.Ask(Commands.SignOnCommand(_instrumentType))
                .ContinueWith(_ =>
                {
                    Context.System.EventStream.Publish(HandshakeState.Idle);
                    Context.System.EventStream.Publish(ConnectionState.Linked);
                });            
        }

        protected async virtual Task WakeUpInstrument()
        {
            Context.System.EventStream.Publish(HandshakeState.WakingItUp);

            _portActor.Tell(Commands.WakeupOne());
            await _portActor.Ask(Commands.WakeupTwo());
        }
    }

    internal class TryDisconnectCommand
    {
    }

    internal class CancelRequested
    {
    }

    internal class TryConnectCommand
    {
    }
}