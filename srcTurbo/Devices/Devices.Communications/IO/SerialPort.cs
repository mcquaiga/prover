using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RJCP.IO.Ports;

namespace Devices.Communications.IO
{
    public sealed class SerialPort : ICommPort
    {
        public static List<int> BaudRates = new List<int> {300, 600, 1200, 2400, 4800, 9600, 19200, 38400};

        private IObservable<char> _dataStream;
        private readonly SerialPortStream _serialStream;
        private ISubject<string> _dataSent = new Subject<string>();
        private IDisposable _dataReceivedConnection;

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

            DataReceived = DataReceivedObservable().Publish();
            _dataReceivedConnection = DataReceived.Connect();
        }

        #region Public

        #region Properties

        public int BaudRate => _serialStream.BaudRate;
        public IConnectableObservable<char> DataReceived { get; private set; }
        public IObservable<string> DataSent => _dataSent;
        public string Name => _serialStream.PortName;

        public int TimeoutMs => _serialStream.ReadTimeout;

        #endregion

        #region Methods

        public Task Close()
        {
            return Task.Run(() => _serialStream.Close());
        }

        public void Dispose()
        {
            _serialStream?.Close();
            _serialStream?.Dispose();
            _dataSent?.OnCompleted();
        }

        public bool IsOpen()
        {
            return _serialStream.IsOpen;
        }

        public async Task Open(CancellationToken ct = new CancellationToken())
        {
            if (_serialStream.IsOpen)
                return;

            _serialStream.Open();
            _serialStream.DiscardInBuffer();
            _serialStream.DiscardOutBuffer();
        }

        public async Task Send(string data)
        {
            if (!_serialStream.IsOpen)
                await Open();

            _serialStream.DiscardOutBuffer();
            _dataSent.OnNext(data);

            var buffer = Encoding.ASCII.GetBytes(data);
            await _serialStream.WriteAsync(buffer, 0, buffer.Length);
        }

        public static IEnumerable<string> GetPortNames()
        {
            return SerialPortStream.GetPortNames();
        }

        #endregion

        #endregion

        #region Private

        private IObservable<char> DataReceivedObservable()
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
        
        #endregion

        public delegate SerialPort Factory(string portName, int baudRate, int timeoutMs = 250);
    }
}