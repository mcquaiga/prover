using System;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Prover.Core.ExternalDevices.DInOutBoards;

namespace Prover.Core.Communication
{
    public class TachometerService : IDisposable
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly IDInOutBoard _outputBoard;
        private readonly SerialPort _serialPort;

        public TachometerService(string portName, IDInOutBoard outputBoard)
        {
            _serialPort = new SerialPort(portName, 9600);
            _outputBoard = outputBoard;
        }

        public async Task ResetTach()
        {
            await Task.Run(() =>
            {
                _outputBoard.StartMotor();
                Thread.Sleep(500);
                _outputBoard.StopMotor();
                Thread.Sleep(100);
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
                    _serialPort.WriteLine(((char) 13).ToString());
                    _serialPort.DiscardInBuffer();
                    Thread.Sleep(500);

                    var tachString = _serialPort.ReadExisting();
                    Log.Info($"Read data from Tach: {tachString}");
                    return ParseTachValue(tachString);
                }
                catch (Exception ex)
                {
                    Log.Warn(ex, "An error occured connecting to the tachometer.");
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
            value = value.Substring(value.IndexOf("D0", StringComparison.Ordinal) + 2);
            var regEx = new Regex(pattern, RegexOptions.IgnoreCase);
            if (int.TryParse(regEx.Match(value).Value, out result))
            {
                return result;
            }
            return -1;
        }

        public void Dispose()
        {
            _serialPort.Close();
            _outputBoard.Dispose();
        }
    }
}