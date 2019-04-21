using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Devices.Communications.IO
{
    public interface ICommPort : IDisposable
    {
        #region Public Properties

        IConnectableObservable<char> DataReceivedObservable { get; }

        ISubject<string> DataSentObservable { get; }

        string Name { get; }

        #endregion Public Properties

        #region Public Methods

        Task Close();

        ICommPort CreateNew();

        bool IsOpen();

        Task Open(CancellationToken ct);

        Task Send(string data);

        #endregion Public Methods
    }
}