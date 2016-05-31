using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Prover.CommProtocol.Common.IO
{
    public abstract class CommPort : IDisposable
    {
        public Subject<char> ReceivedSubject { get; } = new Subject<char>();
        protected IObservable<char> DataReceivedObservable { get; set; }

        public abstract bool IsOpen();
        public abstract Task OpenAsync();
        public abstract Task CloseAsync();
        public abstract Task SendAsync(string data);

        public virtual void Dispose()
        {
            ReceivedSubject.Dispose();
            DataReceivedObservable = null;
        }
    }
}
