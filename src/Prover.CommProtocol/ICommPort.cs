using Akka.Actor;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.CommProtocol
{
    public interface ICommPort : IDisposable
    {
        bool IsOpen { get; }
        void Open();
        void Close();
        IObservable<EventPattern<SerialDataReceivedEventArgs>> DataReceivedObservable { get; }
        void SendDataToPort(string command);
        void DiscardInBuffer();
    }

    public class SerialCommPort : ReceiveActor, ICommPort
    {
        private readonly SerialPort _port;
        public IObservable<EventPattern<SerialDataReceivedEventArgs>> DataReceivedObservable { get; private set; }
        public static event EventHandler<SerialDataReceivedEventArgs> DataReceivedEventHandler;

        public SerialCommPort(string portName, int baudRate, int timeout)
        {
            _port = new SerialPort(portName, baudRate);

            DataReceivedObservable = Observable.FromEventPattern<SerialDataReceivedEventArgs>(
                handler => DataReceivedEventHandler += handler,
                handler => DataReceivedEventHandler -= handler);

            _port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedEventHandler);
        }

        public void Close()
        {
            _port.Close();
        }

        public void DiscardInBuffer()
        {
            _port.DiscardInBuffer();
        }

        public bool IsOpen { get { return _port.IsOpen; } }

        public void Open()
        {
            if (!IsOpen)
                _port.Open();
        }        

        public void SendDataToPort(string command)
        {
            _port.Write(command);
        }

        public void Dispose()
        {
            _port.Dispose();
        }
    }
}
