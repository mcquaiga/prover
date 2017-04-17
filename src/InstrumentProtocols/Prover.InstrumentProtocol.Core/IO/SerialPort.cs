using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using RJCP.IO.Ports;
using Parity = RJCP.IO.Ports.Parity;
using SerialDataReceivedEventArgs = RJCP.IO.Ports.SerialDataReceivedEventArgs;
using StopBits = RJCP.IO.Ports.StopBits;

namespace Prover.InstrumentProtocol.Core.IO
{
    public sealed class SerialCommPort : IDisposable, ICommPort
    {
        public static List<int> BaudRates = new List<int> {300, 600, 1200, 2400, 4800, 9600, 19200, 38400};

        private static List<string> _commPorts;
        private SerialPortStream _serialStream;
        private IDisposable _dataReceivedConnectable;

        public SerialCommPort(string portName, int baudRate, int timeoutMs = 250, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One)
        {
            if (string.IsNullOrEmpty(portName))
                throw new ArgumentNullException(nameof(portName));

            if (!SerialPortStream.GetPortNames().ToList().Contains(portName))
                throw new ArgumentException($"{portName} does not exist.");

            if (!BaudRates.Contains(baudRate))
                throw new ArgumentException(
                    $"Baud rate is invalid. Must be one of the following: {string.Join(",", BaudRates)}");

            try
            {
                _serialStream = new SerialPortStream
                {
                    PortName = portName,
                    BaudRate = baudRate,
                    DataBits = dataBits,
                    Parity = parity,
                    StopBits = stopBits,
                    ReadTimeout = timeoutMs,
                    WriteTimeout = timeoutMs
                };

                DataSentObservable = new Subject<string>();
                DataReceivedObservable = DataReceived().Publish();
                Open();
            }
            catch (Exception ex)
            {
                Dispose();
                throw;
            }
        }

        #region Properties
        public int BaudRate
        {
            get { return _serialStream.BaudRate; }
            set { _serialStream.BaudRate = value; }
        }
        public int DataBits
        {
            get { return _serialStream.DataBits; }
            set { _serialStream.DataBits = value; }
        }
        public bool DtrEnable
        {
            get { return _serialStream.DtrEnable; }
            set { _serialStream.DtrEnable = value; }
        }
        public Parity Parity
        {
            get { return _serialStream.Parity; }
            set { _serialStream.Parity = value; }
        }

        public string PortName
        {
            get { return _serialStream.PortName; }
            set { _serialStream.PortName = value; }
        }

        public bool RtsEnable
        {
            get { return _serialStream.RtsEnable; }
            set { _serialStream.RtsEnable = value; }
        }

        public StopBits StopBits
        {
            get { return _serialStream.StopBits; }
            set { _serialStream.StopBits = value; }
        }
        public bool IsDisposed => _serialStream?.IsDisposed ?? true;
        public bool IsOpen => !_serialStream?.IsDisposed ?? _serialStream?.IsOpen ?? false;
        public string Name => _serialStream.PortName;
        #endregion

        public IConnectableObservable<char> DataReceivedObservable { get; protected set; }
        public ISubject<string> DataSentObservable { get; protected set; }

        public static IObservable<string> PortsWatcherObservable()
        {
            return Observable.Create<string>(observer =>
            {
                _commPorts = SerialPort.GetPortNames().ToList();
                return Observable
                    .Interval(TimeSpan.FromSeconds(1))
                    .Subscribe(
                        _ =>
                        {
                            var ports = SerialPort.GetPortNames().ToList();
                            if (!_commPorts.SequenceEqual(ports))
                                ports.ForEach(observer.OnNext);
                        });
            });
        }

        #region Methods
        public void Close()
        {
            _serialStream?.Close();
            _dataReceivedConnectable?.Dispose();
        }
        #endregion

        public void InitializePort(string portName, int baudRate, int timeoutMs = 250, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One)
        {
           
            
        }

        public void Open()
        {
            if (_serialStream.IsOpen) return;

            _serialStream.Open();
            _dataReceivedConnectable = DataReceivedObservable?.Connect();
        }

        public async Task Send(string data)
        {
            Open();

            _serialStream.DiscardInBuffer();
            _serialStream.DiscardOutBuffer();

            var buffer = Encoding.ASCII.GetBytes(data).ToArray();
            await _serialStream.WriteAsync(buffer, 0, buffer.Length);

            DataSentObservable.OnNext(data);
        }

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


        public void Dispose()
        {
            Close();
            _serialStream?.Dispose();
            _serialStream = null;
            DataSentObservable.OnCompleted();
        }
    }
}