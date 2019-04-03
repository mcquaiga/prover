namespace Prover.Core.ExternalDevices
{
    using NLog;
    using Prover.Core.ExternalDevices.DInOutBoards;
    using System;
    using System.IO.Ports;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="TachometerService" />
    /// </summary>
    public class TachometerService : IDisposable
    {
        #region Fields

        /// <summary>
        /// Defines the Log
        /// </summary>
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Defines the _outputBoard
        /// </summary>
        private readonly IDInOutBoard _outputBoard;

        /// <summary>
        /// Defines the _serialPort
        /// </summary>
        private readonly SerialPort _serialPort;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TachometerService"/> class.
        /// </summary>
        /// <param name="portName">The portName<see cref="string"/></param>
        /// <param name="outputBoard">The outputBoard<see cref="IDInOutBoard"/></param>
        public TachometerService(string portName, IDInOutBoard outputBoard)
        {
            _serialPort = null;

            if (SerialPort.GetPortNames().Contains(portName))
            {
                _serialPort = new SerialPort(portName, 9600);
            }

            _outputBoard = outputBoard;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The ParseTachValue
        /// </summary>
        /// <param name="value">The value<see cref="string"/></param>
        /// <returns>The <see cref="int"/></returns>
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

        /// <summary>
        /// The Dispose
        /// </summary>
        public void Dispose()
        {
            _serialPort?.Close();
            _serialPort?.Dispose();
            _outputBoard?.Dispose();
        }

        /// <summary>
        /// The ReadTach
        /// </summary>
        /// <returns>The <see cref="Task{int}"/></returns>
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
                    _serialPort.Write(((char)13).ToString());
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

        /// <summary>
        /// The ResetTach
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
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

            Thread.Sleep(2000);
        }

        #endregion
    }
}
