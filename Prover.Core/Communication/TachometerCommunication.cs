using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
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
                if (!_serialPort.IsOpen()) _serialPort.OpenPort();

                _serialPort.DiscardInBuffer();
                _serialPort.SendDataToPort("@D0");
                _serialPort.SendDataToPort(((char)13).ToString());
                _serialPort.DiscardInBuffer();
                System.Threading.Thread.Sleep(500);

                var tachString = _serialPort.ReceiveDataFromPort();
                _log.Info(string.Format("Read data from Tach: {0}", tachString));
                var tachReading = ParseTachValue(tachString);
                _log.Info(string.Format("Tach Reading: {0}", tachReading));
                return tachReading;
            });
        }

        public static int ParseTachValue(string value)
        {
            if (value.Length < 1) return 0;
            var index = value.LastIndexOf(Environment.NewLine, System.StringComparison.Ordinal);

            if (index == (value.Length - 2)) value = value.Substring(0, index);

            int returnValue;
            index = value.LastIndexOf(Environment.NewLine, System.StringComparison.Ordinal);
            if (Int32.TryParse(value.Substring(index + 1, (value.Length - 1) - index).Trim(), out returnValue))
                return returnValue;

            return 0;
        }

        public void Dispose()
        {
            _serialPort.ClosePort();
            _outputBoard.Dispose();
        }
    }
}
