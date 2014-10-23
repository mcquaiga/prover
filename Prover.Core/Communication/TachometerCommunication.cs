using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.SerialProtocol;
using Prover.Core.Extensions;

namespace Prover.Core.Communication
{
    public class TachometerCommunication : IDisposable
    {
        private SerialPort _serialPort;
        private DataAcqBoard _outputBoard;

        public TachometerCommunication(string portName)
        {
            _serialPort = new SerialPort(portName, BaudRateEnum.b9600);
            _outputBoard = new DataAcqBoard(0, 0, 1);
        }

        public async Task ResetTach()
        {
            await Task.Run(() =>
            {
                _outputBoard.StartMotor();
                System.Threading.Thread.Sleep(500);
                _outputBoard.StopMotor();
                System.Threading.Thread.Sleep(100);
            });
        }

        public async Task<int> ReadTach()
        {
            return await Task.Run(() =>
            {
                if (!_serialPort.IsOpen()) _serialPort.OpenPort();

                _serialPort.DiscardInBuffer();
                _serialPort.SendDataToPort("@D0");
                _serialPort.SendDataToPort(((char) 13).ToString());
                _serialPort.DiscardInBuffer();

                System.Threading.Thread.Sleep(300);

                var tachString = _serialPort.ReceiveDataFromPort();

                return ParseTachValue(tachString);
            });
        }

        private static int ParseTachValue(string value)
        {
            return Convert.ToInt32(value.Trim(Convert.ToChar(value.Right(value.Length - value.IndexOf((char)13, value.IndexOf((char)13) + 1) - 1))));
        }

        public void Dispose()
        {
            _serialPort.ClosePort();
            _outputBoard.Dispose();
        }
    }
}
