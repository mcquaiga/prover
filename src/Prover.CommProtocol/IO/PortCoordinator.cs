using Akka.Actor;
using Prover.CommProtocol.Communications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prover.CommProtocol.IO
{
    public class PortCoordinator : ReceiveActor, IWithUnboundedStash
    {
        private CommPort _commPort;

        public PortCoordinator(CommPort commPort)
        {
            _commPort = commPort;
            Idle();
        }
        public IStash Stash { get; set; }

        private void Idle()
        {
            Receive<string>(data =>
            {
                BecomeStacked(stash =>
                {
                    Receive<string>(_ => Stash.Stash());
                });

                var response = -1;
                _commPort.SendDataToPort(data);
                using (_commPort.DataReceivedObservable.GetResponseCode().Subscribe(_ => response = _.Item1))
                {
                    if (data != Commands.WakeupOne())
                    {
                        while (response < 0) { }

                        Sender.Tell(response);
                    }                      
                }

                UnbecomeStacked();
                Thread.Sleep(150);
                Stash.Unstash();
            });
        }
    }
}
