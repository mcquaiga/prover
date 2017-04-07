using System;
using System.Threading.Tasks;

namespace Prover.CommProtocol.Common.IO
{
    public interface ICommPort
    {
        string Name { get; }
        Task Close();
        IObservable<char> DataReceived();
        bool IsOpen();
        Task Open();
        Task Send(string data);
    }
}