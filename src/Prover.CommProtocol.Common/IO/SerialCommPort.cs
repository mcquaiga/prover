using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prover.CommProtocol.Common.IO
{
    public class SerialCommPort : CommPort
    {
        public static List<int> BaudRates = new List<int> {300, 600, 1200, 2400, 4800, 9600, 19200, 38400};
        private readonly SerialPort _serialPort;

        public SerialCommPort(SerialPort serialPort)
        {
            _serialPort = serialPort;

            DataReceivedObservable = DataReceived().Publish();
            DataReceivedObservable.Connect();

            DataSentObservable = new Subject<string>();
        }

        public sealed override IConnectableObservable<char> DataReceivedObservable { get; protected set; }
        public sealed override ISubject<string> DataSentObservable { get; protected set; }

        public SerialCommPort(string portName, int baudRate, int timeout = ReadWriteTimeoutMs)
            : this(CreateSerialPort(portName, baudRate, timeout))
        {
        }

        public override string Name => _serialPort.PortName;

        public override async Task OpenAsync()
        {
            if (_serialPort.IsOpen) return;

            var tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(OpenPortTimeoutMs);

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

        public override void Send(string data)
        {
            OpenAsync().Wait();

            _serialPort.DiscardOutBuffer();

            var content = new List<byte>();
            content.AddRange(Encoding.ASCII.GetBytes(data));

            var buffer = content.ToArray();
            //_serialPort.Write(buffer, 0, buffer.Length);
            _serialPort.Write(data);
            DataSentObservable.OnNext(data);
        }

        public override bool IsOpen() => _serialPort.IsOpen;

        public override async Task CloseAsync()
        {
            if (_serialPort.IsOpen)
                await Task.Run(() => _serialPort.Close());
        }

        protected sealed override IObservable<char> DataReceived()
        {
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
                    _serialPort.DiscardInBuffer();
                    return chars;
                });
        }

        public override void Dispose()
        {
            CloseAsync().ContinueWith(_ => _serialPort.Dispose());
        }

        public override string ToString()
            =>
                $"Serial = {_serialPort.PortName}; Baud Rate = {_serialPort.BaudRate}; Timeout = {_serialPort.ReadTimeout}";

        private static SerialPort CreateSerialPort(string portName, int baudRate, int timeout)
        {
            if (portName == null) throw new ArgumentNullException(nameof(portName));
            if (!SerialPort.GetPortNames().Contains(portName)) throw new Exception($"{portName} does not exist.");

            if (!BaudRates.Contains(baudRate)) throw new ArgumentException("Baud rate is invalid.");

            var port = new SerialPort(portName, baudRate)
            {
                RtsEnable = true,
                DtrEnable = true,
                ReadTimeout = timeout,
                WriteTimeout = timeout
            };

            return port;
        }
    }
}