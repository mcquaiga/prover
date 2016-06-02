using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using NLog;

namespace Prover.CommProtocol.Common.IO
{
    public abstract class CommPort : IDisposable
    {
        protected const int OpenPortTimeoutMs = 1000;
        protected const int ReadWriteTimeoutMs = 200;
        protected readonly Logger Log = LogManager.GetCurrentClassLogger();

        protected abstract IObservable<char> DataReceived();
        public abstract IConnectableObservable<char> DataReceivedObservable { get; protected set; }
        public abstract ISubject<string> DataSentObservable { get; protected set; }

        public abstract string Name { get; }
        public abstract bool IsOpen();
        public abstract Task OpenAsync();
        public abstract Task CloseAsync();
        public abstract void Send(string data);

        public abstract void Dispose();
    }
}