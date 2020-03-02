//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reactive.Concurrency;
//using System.Reactive.Disposables;
//using System.Reactive.Linq;
//using System.Reactive.Subjects;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using InTheHand.Net;
//using InTheHand.Net.Sockets;
//using Prover.Shared.IO;

//namespace Devices.Communications.IrDa
//{
//    public sealed class IrDAPort : ICommPort
//    {
//        private const int DevicePeerIndex = 0;

//        private const string ServiceName = "IrDA:IrCOMM";

//        private readonly Encoding _encoding;


//        private IrDAClient _client;

//        private IDisposable _dataReceivedObs;

//        private IrDADeviceInfo _device;

//        private IrDAEndPoint _endPoint;

//        public IrDAPort()
//        {
//            try
//            {
//                _encoding = Encoding.ASCII;
//            }
//            catch (ArgumentException)
//            {
//                _encoding = Encoding.ASCII;
//            }

//            Setup(null, 0);

//            DataReceivedObservable = new Subject<char>().Publish();
//            DataReceivedObservable.Connect();

//            DataSentObservable = new Subject<string>();
//        }

//        public IConnectableObservable<char> DataReceivedObservable { get; private set; }

//        public ISubject<string> DataSentObservable { get; }
//        public static string IrDAName => "IrDA";
//        public string Name => IrDAName;

//        public int RetryCount { get; } = 30;

//        public Task Close()
//        {
//            return Task.Run(() => _client.Close());
//        }

//        public ICommPort CreateNew() => new IrDAPort();

//        public void Dispose()
//        {
//            _dataReceivedObs?.Dispose();

//            _client?.Close();
//            _client?.Dispose();
//            _client = null;
//            _endPoint = null;
//        }

//        public static async Task<IEnumerable<string>> GetIrDADevices()
//        {
//            var client = new IrDAClient();

//            var devices = await GetIrDADevices(client)
//                .RunAsync(new CancellationToken())
//                .ToList();

//            var ds = client.DiscoverDevices();

//            return devices.Select(d => d.DeviceName);
//        }

//        public bool IsOpen() => _client.Connected;

//        public async Task Open(CancellationToken ct)
//        {
//            _client?.Close();

//            _client = new IrDAClient();

//            _client.Client.SetSocketOption(
//                IrDASocketOptionLevel.IrLmp, IrDASocketOptionName.NineWireMode, 1);

//            _device = await SelectIrDAPeerInfo(_client, ct);

//            _endPoint = new IrDAEndPoint(_device.DeviceAddress, ServiceName);

//            if (!_client.Connected)
//                await Task.Factory.FromAsync(
//                    _client.BeginConnect(_endPoint, ConnectCallback, _device),
//                    ar =>
//                    {
//                        if (!_client.Connected)
//                            throw new Exception($"Could not connect to IrDA device {_device.DeviceName}");

//                        DataReceivedObservable = DataReceived(_client.GetStream()).Publish();
//                        _dataReceivedObs = DataReceivedObservable.Connect();
//                    });

//            void ConnectCallback(IAsyncResult ar)
//            {
//                _client.EndConnect(ar);
//            }
//        }

//        public async Task Send(string data)
//        {
//            var strm = _client.GetStream();
//            if (strm.CanWrite)
//            {
//                var buffer = _encoding.GetBytes(data);
//                await strm.WriteAsync(buffer, 0, buffer.Length);
//                await strm.FlushAsync();
//                DataSentObservable.OnNext(data);
//            }
//        }

//        public void Setup(string portName, int baudRate, int timeoutMs = 250)
//        {
//            _client = new IrDAClient();
//            _client.Client.SetSocketOption(
//                IrDASocketOptionLevel.IrLmp, IrDASocketOptionName.NineWireMode, 1);
//        }

//        private IObservable<char> DataReceived(Stream sourceStream)
//        {
//            var subscribed = 0;
//            var scheduler = Scheduler.Default;
//            return Observable.Create<char>(o =>
//            {
//                // first check there is only one subscriber (multiple stream readers would cause havoc)
//                var previous = Interlocked.CompareExchange(ref subscribed, 1, 0);

//                if (previous != 0)
//                    o.OnError(new Exception(
//                        "Only one subscriber is allowed for each stream."));

//                // we will return a disposable that cleans up both the scheduled task below and the
//                // source stream
//                var dispose = new CompositeDisposable
//                {
//                    Disposable.Create(sourceStream.Dispose)
//                };

//                // use async scheduling to get nice imperative code
//                var schedule = scheduler.ScheduleAsync(async (ctrl, ct) =>
//                {
//                    // store the header here each time
//                    var buffer = new byte[1000];

//                    // loop until cancellation requested
//                    while (!ct.IsCancellationRequested)
//                        if (sourceStream.CanRead)
//                        {
//                            int numRead;
//                            try
//                            {
//                                numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length, ct);
//                            }
//                            catch (OperationCanceledException)
//                            {
//                                throw;
//                            }
//                            catch (Exception ex)
//                            {
//                                // pass through any problem in the stream and quit
//                                o.OnError(new InvalidDataException("Error in stream.", ex));
//                                return;
//                            }

//                            ct.ThrowIfCancellationRequested();

//                            var chars = _encoding.GetChars(buffer, 0, numRead);
//                            foreach (var c in chars) o.OnNext(c);
//                        }

//                    // wrap things up
//                    ct.ThrowIfCancellationRequested();
//                    o.OnCompleted();
//                });

//                // return the suscription handle
//                dispose.Add(schedule);
//                return dispose;
//            });
//        }

//        private static IObservable<IrDADeviceInfo> GetIrDADevices(IrDAClient client)
//        {
//            return Observable.Create<IrDADeviceInfo[]>(
//                    o => Observable.ToAsync(client.DiscoverDevices)().Subscribe(o))
//                .SelectMany(i => i);
//        }

//        private async Task<IrDADeviceInfo> SelectIrDAPeerInfo(IrDAClient client, CancellationToken ct)
//        {
//            var tryNumber = 0;

//            return await Task.Run(async () =>
//            {
//                do
//                {
//                    var devices = client.DiscoverDevices();
//                    if (devices.Length > DevicePeerIndex)
//                        return devices[DevicePeerIndex];

//                    tryNumber++;
//                    await Task.Delay(1000, ct);
//                } while (tryNumber < RetryCount);

//                throw new Exception("No IrDA devices found.");
//            }, ct);
//        }
//    }
//}