using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Prover.CommProtocol.Common.Messaging;

namespace Prover.CommProtocol.Common.IO
{
    public class SerialCommPort : CommPort
    {
        public static List<int> BaudRates = new List<int> {300, 600, 1200, 2400, 4800, 9600, 19200, 38400};
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly int _openPortTimeoutMs;
        private readonly SerialPort _serialPort;

        public SerialCommPort(SerialPort serialPort, int timeout = 1000)
        {
            _serialPort = serialPort;

            _openPortTimeoutMs = timeout;

            DataReceivedObservable = CreateReceiveObservable();
            DataReceivedObservable.Subscribe(ReceivedSubject);

            ResponseProcessors.MessageProcessor.ResponseObservable(ReceivedSubject)
                .Subscribe(msg => { _log.Debug($"Incoming >> {msg}"); });
        }

        public SerialCommPort(string portName, int baudRate, int timeout = 1000)
            : this(CreateSerialPort(portName, baudRate, timeout), timeout)
        {
        }

        public override async Task OpenAsync()
        {
            if (_serialPort.IsOpen) return;

            var tokenSource = new CancellationTokenSource(_openPortTimeoutMs);
            tokenSource.CancelAfter(1000);

            await Task.Run(() => _serialPort.Open(), tokenSource.Token)
                .ContinueWith(result =>
                {
                    if (result.IsFaulted)
                    {
                        if (result.Exception != null) throw result.Exception;
                        throw new Exception("Unknown error occured opening the serial port.");
                    }

                    if (result.IsCanceled)
                    {
                        throw new TimeoutException("Opening the Comm port timed out.");
                    }
                }, tokenSource.Token);
        }

        public override async Task SendAsync(string data)
        {
            await Task.Run(async () =>
            {
                await OpenAsync();

                _log.Debug($"Outgoing >> {_serialPort.PortName} - [{data}]");

                _serialPort.DiscardOutBuffer();

                var content = new List<byte>();
                content.AddRange(Encoding.ASCII.GetBytes(data));

                var buffer = content.ToArray();
                _serialPort.Write(buffer, 0, buffer.Length);
            });
        }

        public override bool IsOpen() => _serialPort.IsOpen;

        public override async Task CloseAsync()
        {
            if (_serialPort.IsOpen)
                await Task.Run(() => _serialPort.Close());
        }

        public override void Dispose()
        {
            base.Dispose();
            CloseAsync().ContinueWith(_ => _serialPort.Dispose());
        }

        public override string ToString() => $"Serial = {_serialPort.PortName}; Baud Rate = {_serialPort.BaudRate}; Timeout = {_serialPort.ReadTimeout}";

        private static SerialPort CreateSerialPort(string portName, int baudRate, int timeout = 2000)
        {
            if (portName == null) throw new ArgumentNullException(nameof(portName));
            if (!BaudRates.Contains(baudRate)) throw new ArgumentException("Baud rate is invalid.");

            var port = new SerialPort(portName, baudRate)
            {
                RtsEnable = true,
                DtrEnable = true,
                ReadTimeout = 200,
                WriteTimeout = 150
            };

            return port;
        }

        private IObservable<char> CreateReceiveObservable()
        {
            if (DataReceivedObservable != null) return DataReceivedObservable;

            return Observable.FromEventPattern<SerialDataReceivedEventArgs>(_serialPort, "DataReceived")
                .SelectMany(_ =>
                {
                    if (!_serialPort.IsOpen) return null;

                    var dataLength = _serialPort.BytesToRead;
                    var data = new byte[dataLength];
                    var nbrDataRead = _serialPort.Read(data, 0, dataLength);

                    if (nbrDataRead == 0)
                        return new char[0];

                    var chars = Encoding.ASCII.GetChars(data);
                    return chars;
                });
        }
    }
}