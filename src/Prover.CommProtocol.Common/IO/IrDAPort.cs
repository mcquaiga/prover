using InTheHand.Net;
using InTheHand.Net.Sockets;
using NLog;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prover.CommProtocol.Common.IO
{

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public sealed class IrDAPort : ICommPort
    {
        private const int DevicePeerIndex = 0;
        private const string ServiceName = "IrDA:IrCOMM";
        private readonly Encoding _encoding;
        private IrDAClient _client;
        private IrDAEndPoint _endPoint;
        private IrDADeviceInfo _device;
        private IConnectableObservable<char> _dataReceivedConnectableObservable;
        private IDisposable _dataReceivedObs;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public IrDAPort()
        {
            try
            {
                _encoding = Encoding.ASCII;
            }
            catch (ArgumentException)
            {
                _encoding = Encoding.ASCII;
            }

            _client = new IrDAClient();
            _client.Client.SetSocketOption(
                IrDASocketOptionLevel.IrLmp, IrDASocketOptionName.NineWireMode, 1);
            
            _dataReceivedConnectableObservable = new Subject<char>().Publish();
            DataReceivedObservable.Connect();

            DataSentObservable = new Subject<string>();
        }

        public int RetryCount { get; } = 30;

        public string Name => "IrDA";

        public IConnectableObservable<char> DataReceivedObservable => _dataReceivedConnectableObservable;
        public ISubject<string> DataSentObservable { get; }

        private IObservable<char> DataReceived(Stream sourceStream)
        {
            int subscribed = 0;
            var scheduler = Scheduler.Default;
            return Observable.Create<char>(o =>
            {
                // first check there is only one subscriber
                // (multiple stream readers would cause havoc)
                int previous = Interlocked.CompareExchange(ref subscribed, 1, 0);

                if (previous != 0)
                    o.OnError(new Exception(
                        "Only one subscriber is allowed for each stream."));

                // we will return a disposable that cleans
                // up both the scheduled task below and
                // the source stream
                var dispose = new CompositeDisposable
                {
                    Disposable.Create(sourceStream.Dispose)
                };

                // use async scheduling to get nice imperative code
                var schedule = scheduler.ScheduleAsync(async (ctrl, ct) =>
                {
                    // store the header here each time
                    var buffer = new byte[1000];

                    // loop until cancellation requested
                    while (!ct.IsCancellationRequested)
                    {
                        if (sourceStream.CanRead)
                        {
                            int numRead;
                            try
                            {
                                numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length, ct);
                            }
                            catch (OperationCanceledException)
                            {
                                throw;
                            }
                            catch (Exception ex)
                            {
                                // pass through any problem in the stream and quit
                                o.OnError(new InvalidDataException("Error in stream.", ex));
                                return;
                            }
                            ct.ThrowIfCancellationRequested();

                            var chars = _encoding.GetChars(buffer, 0, numRead);
                            foreach (var c in chars)
                            {
                                o.OnNext(c);
                            }
                        }
                    }
                    // wrap things up
                    ct.ThrowIfCancellationRequested();
                    o.OnCompleted();
                });

                // return the suscription handle
                dispose.Add(schedule);
                return dispose;
            });
        }

        public bool IsOpen()
        {
            return _client.Connected;
        }

        public async Task Open(CancellationToken ct)
        {
            _client = new IrDAClient();
            _client.Client.SetSocketOption(
                IrDASocketOptionLevel.IrLmp, IrDASocketOptionName.NineWireMode, 1);

            //if (_device == null)
                _device = await SelectIrDAPeerInfo(_client, ct);

            //if (_endPoint == null)
                _endPoint = new IrDAEndPoint(_device.DeviceAddress, ServiceName);

            if (!_client.Connected)
            {
                await Task.Factory.FromAsync(
                    _client.BeginConnect(_endPoint, ar =>
                    {
                        try
                        {
                            _client.EndConnect(ar);
                        }
                        catch (Exception ex)
                        {
                            _log.Error(ex);
                            throw;
                        }
                    }, _device),
                    ar =>
                    {
                        if (!_client.Connected) return;                        
                        _dataReceivedConnectableObservable = DataReceived(_client.GetStream()).Publish();
                        _dataReceivedObs = _dataReceivedConnectableObservable.Connect();
                    });
            }
        }

        public async Task Close()
        {
            await Task.Run(() => _client.Close());
        }

        public async Task Send(string data)
        {
            var strm = _client.GetStream();
            if (strm.CanWrite)
            {
                var buffer = _encoding.GetBytes(data);
                await strm.WriteAsync(buffer, 0, buffer.Length);
                await strm.FlushAsync();
                DataSentObservable.OnNext(data);
            }
        }

        public void Dispose()
        {
            _dataReceivedObs?.Dispose();

            _client?.Close();
            _client?.Dispose();
            _client = null;
            _endPoint = null;
        }

        private async Task<IrDADeviceInfo> SelectIrDAPeerInfo(IrDAClient client, CancellationToken ct)
        {
            var tryNumber = 0;

            return await Task.Run(async () =>
            {
                do
                {
                    var devices = client.DiscoverDevices();
                    if (devices.Length > DevicePeerIndex)
                        return devices[DevicePeerIndex];

                    tryNumber++;
                    await Task.Run(() => Thread.Sleep(1000), ct);
                } while (tryNumber < RetryCount);

                throw new Exception("No IrDA devices found.");
            }, ct);

            
        }

        public ICommPort CreateNew()
        {
            return new IrDAPort();
        }
    }
}