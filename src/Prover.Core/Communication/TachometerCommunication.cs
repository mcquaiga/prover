using NLog;
using Prover.Core.ExternalDevices.DInOutBoards;
using Prover.SerialProtocol;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Prover.Core.Communication
{
    public class TachometerCommunicator : IDisposable
    {
        private readonly SerialPort _serialPort;
        private IDInOutBoard _outputBoard;
        private Logger _log = LogManager.GetCurrentClassLogger();

        public TachometerCommunicator(string portName)
        {
            _serialPort = new SerialPort(portName, BaudRateEnum.b9600);
            _outputBoard = DInOutBoardFactory.CreateBoard(0, 0, 1);
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
                    if (!_serialPort.IsOpen()) _serialPort.OpenPort();

                    _serialPort.DiscardInBuffer();
                    _serialPort.SendDataToPort("@D0");
                    _serialPort.SendDataToPort(((char)13).ToString());
                    _serialPort.DiscardInBuffer();
                    System.Threading.Thread.Sleep(500);

                    var tachString = _serialPort.ReceiveDataFromPort();
                    _log.Info(string.Format("Read data from Tach: {0}", tachString));
                    return ParseTachValue(tachString);
                }
                catch (Exception ex)
                {
                    _log.Warn("An error occured connecting to the tachometer.", ex);
                }
                return 0;
            });
        }

        public static int ParseTachValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;

            const string pattern = @"(\d+)";
            int result;
            value = value.Replace("\n", " ").Replace("\r", " ");
            value = value.Substring(value.IndexOf("D0", System.StringComparison.Ordinal) + 2);
            var regEx = new Regex(pattern, RegexOptions.IgnoreCase);
            if (Int32.TryParse(regEx.Match(value).Value, out result))
            {
                return result;
            }
            return -1;
        }

        public void Dispose()
        {
            _serialPort.ClosePort();
            _outputBoard.Dispose();
        }
    }
}
