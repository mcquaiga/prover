using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Prover.CommProtocol.Common.IO;

namespace Prover.CommProtocol.MiHoneywell.Simulator.IO
{
    public class SimulatorPort : CommPort
    {
        public SimulatorPort(string name)
        {
            Name = name;
        }

        protected override IObservable<char> DataReceived()
        {
            return null;
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
