using NLog;
using RJCP.IO.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prover.CommProtocol.Common.IO
{
    public sealed class SerialPort : ICommPort
    {
        public delegate SerialPort Factory(string portName, int baudRate, int timeoutMs = 250);

        public static List<int> BaudRates = new List<int> {300, 600, 1200, 2400, 4800, 9600, 19200, 38400};
        private readonly SerialPortStream _serialStream;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public SerialPort(string portName, int baudRate, int timeoutMs = 250)
        {
            if (string.IsNullOrEmpty(portName))
                throw new ArgumentNullException(nameof(portName));

            if (!SerialPortStream.GetPortNames().ToList().Contains(portName))
                throw new ArgumentException($"{portName} does not exist.");

            if (!BaudRates.Contains(baudRate))
                throw new ArgumentException(
                    $"Baud rate is invalid. Must be one of the following: {string.Join(",", BaudRates)}");

            _serialStream = new SerialPortStream
            {
                PortName = portName,
                BaudRate = baudRate,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One,
                ReadTimeout = timeoutMs,
                WriteTimeout = timeoutMs
            };

            DataReceivedObservable = DataReceived().Publish();
            DataReceivedObservable.Connect();

            DataSentObservable = new Subject<string>();
        }

        public IConnectableObservable<char> DataReceivedObservable { get; }
        public ISubject<string> DataSentObservable { get; }
        public string Name => _serialStream.PortName;

        private IObservable<char> DataReceived()
        {
            return Observable.FromEventPattern<SerialDataReceivedEventArgs>(_serialStream, "DataReceived")
                .SelectMany(_ =>
                {
                    var dataLength = _serialStream.BytesToRead;
                    var data = new byte[dataLength];
                    var nbrDataRead = _serialStream.Read(data, 0, dataLength);
                    if (nbrDataRead == 0)
                        return new char[0];

                    var chars = Encoding.ASCII.GetChars(data);
                    return chars;
                });
        }

        public bool IsOpen() => _serialStream.IsOpen;

        public async Task Open(CancellationToken ct)
        {
            if (_serialStream.IsOpen) return;

            await Task.Run(() => _serialStream.Open(), ct);
        }

        public async Task Close()
        {
            await Task.Run(() => _serialStream.Close());
        }

        public async Task Send(string data)
        {
            _serialStream.DiscardInBuffer();
            _serialStream.DiscardOutBuffer();

            var content = new List<byte>();
            content.AddRange(Encoding.ASCII.GetBytes(data));

            var buffer = content.ToArray();
            await _serialStream.WriteAsync(buffer, 0, buffer.Length);

            DataSentObservable.OnNext(data);
        }

        public void Dispose()
        {
            _serialStream?.Close();
            _serialStream?.Dispose();
        }

        public static IEnumerable<string> GetPortNames()
        {
            return SerialPortStream.GetPortNames();
        }
    }
}