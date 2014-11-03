using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.SerialProtocol;
using Prover.Core.Extensions;

namespace Prover.Core.Communication
{
    public class TachometerCommunication : IDisposable
    {
        private readonly System.IO.Ports.SerialPort _serialPort;
        private DataAcqBoard _outputBoard;

        public TachometerCommunication(string portName)
        {
            _serialPort = new System.IO.Ports.SerialPort(portName, 9600);
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
                try
                {
                    if (!_serialPort.IsOpen) _serialPort.Open();

                    _serialPort.DiscardInBuffer();
                    _serialPort.WriteLine("@D0");
                    //_serialPort.WriteLine(((char)13).ToString(CultureInfo.InvariantCulture));
                    _serialPort.DiscardInBuffer();

                    System.Threading.Thread.Sleep(300);

                    var tachString = _serialPort.ReadLine();

                    return ParseTachValue(tachString);
                }
                catch (Exception)
                {
                    throw;
                }
                
            });
        }

        public static int ParseTachValue(string value)
        {
            if (value.Length < 1) return 0;
            var index = value.LastIndexOf((char) 13);
            return Convert.ToInt32(value.Substring(index + 1, (value.Length - 1) - index).Trim());
        }

        public void Dispose()
        {
            _serialPort.Close();
            _outputBoard.Dispose();
        }
    }
}
