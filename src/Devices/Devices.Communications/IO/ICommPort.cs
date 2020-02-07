using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Devices.Communications.IO
{
    public interface ICommPort : IDisposable
    {
        #region Public Properties

        IConnectableObservable<char> DataReceived { get; }

        IObservable<string> DataSent { get; }

        string Name { get; }

        #endregion

        #region Public Methods

        Task Close();

        bool IsOpen();

        Task Open(CancellationToken ct);

        Task Send(string data);

        #endregion
    }
}