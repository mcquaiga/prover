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
            if (string.IsNullOrEmpty(portName))
            {
                _serialPort = null;
                _outputBoard = null;
            }
            else
            {
                _serialPort = new SerialPort(portName, 9600);
                if (!_serialPort.IsOpen)
                    _serialPort.Open();

                _outputBoard = outputBoard;
            }
               
        }

        public void Dispose()
        {
            _serialPort?.Close();
            _outputBoard?.Dispose();
        }

        public async Task ResetTach()
        {
            await Task.Run(() =>
            {
                if (_serialPort == null)
                    return;

                if (!_serialPort.IsOpen) _serialPort.Open();

                _serialPort.Write($"@T1{(char)13}");
                Thread.Sleep(50);
                _serialPort.Write($"6{(char)13}");
                _serialPort.DiscardInBuffer();
            });

            await Task.Run(() =>
            {
                _outputBoard?.StartMotor();
                Thread.Sleep(500);
                _outputBoard?.StopMotor();
                Thread.Sleep(100);
            });
        }

        public async Task<int> ReadTach()
        {
            if (_serialPort == null)
                return -1;

            return await Task.Run(() =>
            {
                try
                {
                    if (!_serialPort.IsOpen)
                        _serialPort.Open();

                    _serialPort.DiscardInBuffer();
                    _serialPort.Write("@D0");
                    _serialPort.Write(((char) 13).ToString());
                    Thread.Sleep(100);

                    var tachString = _serialPort.ReadExisting();

                    Log.Debug($"Read data from Tach: {tachString}");
                    return ParseTachValue(tachString);
                }
                finally
                {
                    _serialPort.Close();
                }
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
                return result;
            return -1;
        }
    }
}