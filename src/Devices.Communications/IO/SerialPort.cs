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
        #region Public Fields

        public static List<int> BaudRates = new List<int> { 300, 600, 1200, 2400, 4800, 9600, 19200, 38400 };

        #endregion Public Fields

        #region Public Constructors

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

            DataSentObservable = new Subject<string>();
            _dataStream = DataReceived();
        }

        #endregion Public Constructors

        #region Public Delegates

        public delegate SerialPort Factory(string portName, int baudRate, int timeoutMs = 250);

        #endregion Public Delegates

        #region Public Properties

        public IConnectableObservable<char> DataReceivedObservable => _dataStream.Publish();

        public ISubject<string> DataSentObservable { get; }

        public string Name => _serialStream.PortName;

        private IObservable<char> _dataStream;

        #endregion Public Properties

        #region Public Methods

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
            DataSentObservable.OnCompleted();
        }

        public bool IsOpen() => _serialStream.IsOpen;

        public async Task Open(CancellationToken ct = new CancellationToken())
        {
            if (_serialStream.IsOpen)
                return;

            DataReceivedObservable.Connect();

            await Task.Run(() => _serialStream.Open(), ct);
        }

        public async Task Send(string data)
        {
            if (!_serialStream.IsOpen)
                await Open();

            _serialStream.DiscardInBuffer();
            _serialStream.DiscardOutBuffer();

            var buffer = Encoding.ASCII.GetBytes(data);
            await _serialStream.WriteAsync(buffer, 0, buffer.Length);

            DataSentObservable.OnNext(data);
        }

        #endregion Public Methods

        #region Private Fields

        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        private readonly SerialPortStream _serialStream;

        #endregion Private Fields

        #region Private Methods

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

        #endregion Private Methods
    }
}