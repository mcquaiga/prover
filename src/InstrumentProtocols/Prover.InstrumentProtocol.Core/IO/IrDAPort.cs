using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InTheHand.Net;
using InTheHand.Net.Sockets;
using RJCP.IO.Ports;

namespace Prover.InstrumentProtocol.Core.IO
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public sealed class IrDAPort : ICommPort
    {
        private const int DevicePeerIndex = 0;
        private const string ServiceName = "IrDA:IrCOMM";
        private readonly Encoding _encoding;
        private IrDAClient _client;
        private IrDAEndPoint _endPoint;

        public IrDAPort()
        {
            try
            {
                _encoding = Encoding.GetEncoding("x-IAS");
            }
            catch (ArgumentException)
            {
                _encoding = Encoding.ASCII;
            }

            LoadDevice();

            DataReceivedObservable = DataReceived().Publish();
            DataSentObservable = new Subject<string>();
        }

        public IConnectableObservable<char> DataReceivedObservable { get; protected set; }
        public ISubject<string> DataSentObservable { get; protected set; }

        public string Name => "IrDA";

        public int RetryCount { get; } = 10;

        public async void Close()
        {
            await Task.Run(() => _client.Close()); 
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool IsOpen => _client.Connected;

        public async void Open()
        {
            if (IsOpen) return;

            var tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(5000);

            await Task.Run(() => { _client.Connect(_endPoint); }, tokenSource.Token);
            DataReceivedObservable.Connect();
        }

        public async Task Send(string data)
        {
            if (!IsOpen)
                Open();

            await Task.Run(() =>
            {
                using (var stream = _client.GetStream())
                {
                    // Sleep for 5 seconds to give the IrDA "connected" icon lots of time to 
                    // appear.
                    Thread.Sleep(2500);
                    if (stream.CanWrite)
                    {
                        var content = new List<byte>();
                        content.AddRange(Encoding.ASCII.GetBytes(data));

                        var buffer = content.ToArray();
                        stream.Write(buffer, 0, buffer.Length);

                        DataSentObservable.OnNext(data);
                    }
                }
            });
        }

        private void LoadDevice()
        {
            _client = new IrDAClient();

            _client.Client.SetSocketOption(
                IrDASocketOptionLevel.IrLmp, IrDASocketOptionName.NineWireMode,
                1);

            var device = SelectIrDAPeerInfo(_client);
            _endPoint = new IrDAEndPoint(device.DeviceAddress, "IrDA:IrCOMM");
        }

        private IrDADeviceInfo SelectIrDAPeerInfo(IrDAClient client)
        {
            var tryNumber = 0;

            do
            {
                var devices = client.DiscoverDevices();
                if (devices.Count() > DevicePeerIndex)
                    return devices[DevicePeerIndex];

                tryNumber++;
                Thread.Sleep(5000);
            } while (tryNumber < RetryCount);

            throw new Exception("No IrDA devices found.");
        }

        private IObservable<char> DataReceived()
        {
            return Observable.Defer(async () =>
            {
                var stream = _client.GetStream();

                var buffer = new byte[1024];
                var readBytes = await stream.ReadAsync(buffer, 0, buffer.Length);
                var chars = Encoding.ASCII.GetChars(buffer);
                return chars.Take(readBytes).ToObservable();
            });
        }
    }
}