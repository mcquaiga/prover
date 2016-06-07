using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Prover.CommProtocol.Common.IO
{
    public class SimulatorPort : CommPort
    {
        public SimulatorPort(string name)
        {
            Name = name;
        }

        protected override IObservable<char> DataReceived()
        {
            return 
        }

        public override IConnectableObservable<char> DataReceivedObservable { get; protected set; }
        public override ISubject<string> DataSentObservable { get; protected set; }
        public override string Name { get; }

        public override bool IsOpen()
        {
            return true;
        }

        public override async Task OpenAsync()
        {
            await Task.Run(() => Log.Info("Open simulator port."));
        }

        public override async Task CloseAsync()
        {
            await Task.Run(() => Log.Info("Closing simulator port."));
        }

        public override void Send(string data)
        {
            
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
