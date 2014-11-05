using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Targets;
using Prover.SerialProtocol;
using Prover.Core.Extensions;

namespace Prover.Core.Communication
{
    public class TachometerCommunication : IDisposable
    {
        private readonly SerialPort _serialPort;
        private DataAcqBoard _outputBoard;
        private Logger _log = LogManager.GetCurrentClassLogger();

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
                string tachString = string.Empty;
                try
                {
                    if (!_serialPort.IsOpen()) _serialPort.OpenPort();

                    _serialPort.DiscardInBuffer();
                    _serialPort.SendDataToPort("@D0");
                    _serialPort.SendDataToPort(((char)13).ToString());
                    _serialPort.DiscardInBuffer();
                    System.Threading.Thread.Sleep(500);

                    tachString = _serialPort.ReceiveDataFromPort();
                    _log.Debug(string.Format("Raw data from Tach: {0}", tachString));
                    var tachReading = ParseTachValue(tachString);
                    _log.Debug(string.Format("Tach Reading: {0}", tachReading));
                    return tachReading;
                }
                catch (Exception ex)
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
            _serialPort.ClosePort();
            _outputBoard.Dispose();
        }
    }
}
