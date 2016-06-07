using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InTheHand.Net;
using InTheHand.Net.Sockets;

namespace Prover.CommProtocol.Common.IO
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class IrDAPort : CommPort
    {
        private const int DevicePeerIndex = 0;
        private const string ServiceName = "IrDA:IrCOMM";
        private readonly Encoding _encoding;
        private readonly IrDAClient _client;
        private IrDAEndPoint _endPoint;

        public IrDAPort()
        {
            _client = new IrDAClient(ServiceName);
            try
            {
                _encoding = Encoding.GetEncoding("x-IAS");
            }
            catch (ArgumentException)
            {
                _encoding = Encoding.ASCII;
            }

            LoadDevice();
        }
        
        public override string Name => "IrDA";

        protected sealed override IObservable<char> DataReceived()
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

        public override IConnectableObservable<char> DataReceivedObservable { get; protected set; }
        public override ISubject<string> DataSentObservable { get; protected set; }

        public override bool IsOpen()
        {
            return _client.Connected;
        }

        public override async Task OpenAsync()
        {
            if (IsOpen()) return;

            var tokenSource = new CancellationTokenSource(150);
            tokenSource.CancelAfter(OpenPortTimeoutMs);

            await Task.Run(() =>
            {
                _client.Connect(_endPoint);
                // Sleep for 5 seconds to give the IrDA "connected" icon lots of time to 
                // appear.
                Thread.Sleep(5000);
            }, tokenSource.Token);
        }

        public override async void CloseAsync()
        {
            await Task.Run(() => _client.Close());
        }

        public override void Send(string data)
        {
            OpenAsync().Wait();

            using (var stream = _client.GetStream())
            {
                if (stream.CanWrite)
                {
                    var content = new List<byte>();
                    content.AddRange(Encoding.ASCII.GetBytes(data));

                    var buffer = content.ToArray();
                    stream.Write(buffer, 0, buffer.Length);

                    Log.Debug($"[{ServiceName}] Outgoing >> - {ControlCharacters.Prettify(data)}");
                }
            }
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        private void LoadDevice()
        {
            _client.Client.SetSocketOption(
                IrDASocketOptionLevel.IrLmp,
                IrDASocketOptionName.NineWireMode,
                1);

            var device = SelectIrDAPeerInfo(_client);

            _endPoint = new IrDAEndPoint(device.DeviceAddress, ServiceName);
        }

        private IrDADeviceInfo SelectIrDAPeerInfo(IrDAClient client)
        {
            var devices = client.DiscoverDevices();

            if (!devices.Any()) throw new Exception("No IrDA devices found.");

            return devices[DevicePeerIndex];
        }
    }
}