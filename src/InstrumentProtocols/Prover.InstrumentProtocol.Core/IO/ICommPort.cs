using System;
using System.IO;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using RJCP.IO.Ports;

namespace Prover.InstrumentProtocol.Core.IO
{
    public interface ICommPort : IDisposable
    {
        string Name { get; }
        void Close();
        bool IsOpen { get; }
        IConnectableObservable<char> DataReceivedObservable { get; }
        ISubject<string> DataSentObservable { get; }
        void Open();
        Task Send(string data);
    }
}