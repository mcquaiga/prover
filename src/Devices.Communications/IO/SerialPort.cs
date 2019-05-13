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

namespace Devices.Communications.IO
{
    public sealed class SerialPort : ICommPort
    {
        public static List<int> BaudRates = new List<int> { 300, 600, 1200, 2400, 4800, 9600, 19200, 38400 };

        public int BaudRate { get; private set; }
        public IConnectableObservable<char> DataReceivedObservable { get; private set; }
        public ISubject<string> DataSentObservable { get; private set; }
        public string Name => _serialStream.PortName;

        public string PortName { get; private set; }
        public int TimeoutMs { get; private set; }

        public SerialPort()
        {
            _serialStream = new SerialPortStream();
        }

        public SerialPort(string portName, int baudRate, int timeoutMs = 250)
        {
            if (string.IsNullOrEmpty(portName))
                throw new ArgumentNullException(nameof(portName));

            if (!SerialPortStream.GetPortNames().ToList().Contains(portName))
                throw new ArgumentException($"{portName} does not exist.");

            if (!BaudRates.Contains(baudRate))
                throw new ArgumentException(
                    $"Baud rate is invalid. Must be one of the following: {string.Join(",", BaudRates)}");

            Setup(portName, baudRate, timeoutMs);
        }

        public delegate SerialPort Factory(string portName, int baudRate, int timeoutMs = 250);

        public static IEnumerable<string> GetPortNames()
        {
            return SerialPortStream.GetPortNames();
        }

        public Task Close()
        {
            return Task.Run(() => _serialStream.Close());
        }

        public ICommPort CreateNew()
        {
            return new SerialPort(_serialStream.PortName, _serialStream.BaudRate, _serialStream.ReadTimeout);
        }

        public void Dispose()
        {
            _serialStream?.Close();
            _serialStream?.Dispose();
            DataSentObservable?.OnCompleted();
            _receivedStreamDisposable?.Dispose();
        }

        public bool IsOpen() => _serialStream.IsOpen;

        public async Task Open(CancellationToken ct = new CancellationToken())
        {
            if (_serialStream.IsDisposed)
                Setup(PortName, BaudRate, TimeoutMs);

            if (_serialStream.IsOpen)
                return;

            _receivedStreamDisposable = DataReceivedObservable.Connect();

            _serialStream.Open();
            _serialStream.DiscardInBuffer();
            _serialStream.DiscardOutBuffer();
        }

        public async Task Send(string data)
        {
            if (!_serialStream.IsOpen)
                await Open();

            _serialStream.DiscardOutBuffer();

            DataSentObservable.OnNext(data);

            var buffer = Encoding.ASCII.GetBytes(data);
            await _serialStream.WriteAsync(buffer, 0, buffer.Length);
        }

        public void Setup(string portName, int baudRate, int timeoutMs = 250)
        {
            if (_serialStream == null || _serialStream.IsDisposed)
            {
                _serialStream = new SerialPortStream();
            }
            _serialStream = new SerialPortStream();
            if (_serialStream.IsOpen)
                _serialStream.Close();

            PortName = portName;
            BaudRate = baudRate;
            TimeoutMs = timeoutMs;

            _serialStream.PortName = portName;
            _serialStream.BaudRate = baudRate;
            _serialStream.DataBits = 8;
            _serialStream.Parity = Parity.None;
            _serialStream.StopBits = StopBits.One;
            _serialStream.ReadTimeout = timeoutMs;
            _serialStream.WriteTimeout = timeoutMs;

            DataSentObservable = new Subject<string>();
            _dataStream = DataReceived();
            DataReceivedObservable = _dataStream.Publish();
        }

        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private IObservable<char> _dataStream;
        private IDisposable _receivedStreamDisposable;
        private SerialPortStream _serialStream;

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
    }
}