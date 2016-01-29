using Akka.Actor;
using NLog;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prover.CommProtocol.IO
{
    public abstract class CommPort
    {
        public List<int> BaudRates = new List<int> { 300, 600, 1200, 2400, 4800, 9600, 19200, 38400 };

        public IObservable<char> DataReceivedObservable { get; set; }

        public abstract void Write(string data);
    }

    public class SerialCommPort : CommPort
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly SerialPort _port;

        public SerialCommPort(string portName, int baudRate, int timeout)
        {
            if (portName == null) throw new ArgumentNullException(nameof(portName));
            if (!BaudRates.Contains(baudRate)) throw new ArgumentException("Baud rate is invalid.");

            _port = new SerialPort(portName, baudRate);

            DataReceivedObservable = CreateReceiveObservable();
        }

        public override void Write(string data)
        {
            Console.WriteLine("Sending '{0}' to serial port.", data);
            Open();
            _port.Write(data);
        }

        public void Dispose()
        {
            if (_port.IsOpen) _port.Close();
            _port.Dispose();
        }

        public override string ToString()
        {
            return string.Format("Serial = {0}; Baud Rate = {1}", _port.PortName, _port.BaudRate);
        }

        private void Open()
        {
            if (_port.IsOpen) return;

            var cts = new CancellationTokenSource();

            var openTask = Task.Run(() => _port.Open(), cts.Token);
            openTask.Wait();
        }

        private IObservable<char> CreateReceiveObservable()
        {
            if (DataReceivedObservable != null) return DataReceivedObservable;
 
            return Observable.FromEventPattern<SerialDataReceivedEventArgs>(_port, "DataReceived")
                    .SelectMany(_ =>
                    {
                        int dataLength = _port.BytesToRead;
                        byte[] data = new byte[dataLength];
                        int nbrDataRead = _port.Read(data, 0, dataLength);

                        if (nbrDataRead == 0)
                            return new char[0];

                        var chars = Encoding.ASCII.GetChars(data);

                        return chars;
                    });
        }
    }
}
