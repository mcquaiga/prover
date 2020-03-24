using Devices.Communications.IO;
using Prover.Shared.IO;

namespace Devices.Communications.Interfaces
{
    public interface ICommPortFactory
    {
        ICommPort Create(string portName = null, int? baudRate = null);
    }

    public class CommPortFactory : ICommPortFactory
    {
        private static SerialPort _serialPort;
        private static ICommPort _irDaPort;
        private static ICommPort _currentPort;

        public ICommPort Create(string portName = null, int? baudRate = null)
        {
            CleanupPorts();   

            if (!string.IsNullOrEmpty(portName) && baudRate.HasValue)
            {
                _serialPort = new SerialPort(portName, baudRate.Value);
                _currentPort = _serialPort;
            }
            else
            {
                //_irDaPort = new IrDAPort();
                _currentPort = _irDaPort;
            }

            return _currentPort;
        }

        private static void CleanupPorts()
        {
            if (_serialPort != null)
            {
                _serialPort.Close();
                _serialPort.Dispose();
            }

            if (_irDaPort != null)
            {
                _irDaPort.Dispose();
            }

            _currentPort = null;
        }
    }
}