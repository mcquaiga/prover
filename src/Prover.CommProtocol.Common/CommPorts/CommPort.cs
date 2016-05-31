using System;
using System.Threading.Tasks;

namespace Prover.CommProtocol.Common.CommPorts
{
    public abstract class CommPort : IDisposable
    {
        public abstract bool IsOpen();

        public abstract Task OpenAsync();
        public abstract Task CloseAsync();

        public abstract IObservable<char> DataReceivedObservable { get; }
        public abstract Task SendAsync(string data);

        public virtual void Dispose()
        {
            
        }
    }
}
