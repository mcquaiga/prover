using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.ExternalDevices.DInOutBoards;
using RJCP.IO.Ports;

namespace Prover.Application.ExternalDevices
{
    public interface ITachometerService
    {

        /// <summary>
        ///     The ReadTach
        /// </summary>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /></returns>
        Task<int> ReadTach();

        /// <summary>
        ///     The ResetTach
        /// </summary>
        /// <returns>The <see cref="System.Threading.Tasks.Task{TResult}" /></returns>
        Task ResetTach();
    }

    public class NullTachometerService : ITachometerService
    {
        public async Task<int> ReadTach()
        {
            return await Task.Run(() => 0);
        }

        public async Task ResetTach()
        {
            await Task.CompletedTask;
        }
    }

    /// <summary>
    ///     Defines the <see cref="TachometerService" />
    /// </summary>
    public class TachometerService : ITachometerService, IDisposable
    {
        /// <summary>
        ///     Defines the _outputBoard
        /// </summary>
        private readonly IOutputChannel _outputBoard;

        /// <summary>
        ///     Defines the _serialPort
        /// </summary>
        private readonly SerialPortStream _serialPort;

        /// <summary>
        ///     Defines the Log
        /// </summary>
        protected ILogger Logger;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TachometerService" /> class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="portName">The portName<see cref="string" /></param>
        /// <param name="portFactory"></param>
        /// <param name="outputBoard">The outputBoard<see cref="IDInOutBoard" /></param>
        public TachometerService(ILogger logger, string portName, IOutputChannel outputBoard)
        {
            Logger = logger ?? NullLogger.Instance;

            if (string.IsNullOrEmpty(portName) || !SerialPortStream.GetPortNames().Contains(portName))
                throw new ArgumentException($"Could not find serial port {portName} on this computer.");

            _serialPort = new SerialPortStream(portName, 9600);

            _outputBoard = outputBoard;
        }

        /// <summary>
        ///     The Dispose
        /// </summary>
        public void Dispose()
        {
            _serialPort?.Close();
            _serialPort?.Dispose();
            (_outputBoard as IDisposable)?.Dispose();
        }

        public void Setup(string portName)
        {
          
        }

        /// <summary>
        ///     The ReadTach
        /// </summary>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /></returns>
        public async Task<int> ReadTach()
        {
            if (_serialPort == null)
                return -1;


            if (!_serialPort.IsOpen)
                _serialPort.Open();

            _serialPort.DiscardInBuffer();
            _serialPort.Write("@D0");
            _serialPort.Write(((char) 13).ToString());

            await Task.Delay(100);

            var tachString = _serialPort.ReadExisting();
            Logger.LogDebug($"Read data from Tach: {tachString}");

            return ParseTachValue(tachString);
        }

        /// <summary>
        ///     The ResetTach
        /// </summary>
        /// <returns>The <see cref="System.Threading.Tasks.Task{TResult}" /></returns>
        public async Task ResetTach()
        {
            await Task.Run(() =>
            {
                if (_serialPort == null)
                    return;

                if (!_serialPort.IsOpen) _serialPort.Open();

                _serialPort.Write($"@T1{(char) 13}");
                Thread.Sleep(50);
                _serialPort.Write($"6{(char) 13}");
                _serialPort.DiscardInBuffer();
            });

            await Task.Run(() =>
            {
                _outputBoard?.SignalStart();
                Thread.Sleep(500);
                _outputBoard?.SignalStop();
                Thread.Sleep(100);
            });

            //await Task.Delay(2000);
        }

        /// <summary>
        ///     The ParseTachValue
        /// </summary>
        /// <param name="value">The value<see cref="string" /></param>
        /// <returns>The <see cref="int" /></returns>
        private static int ParseTachValue(string value)
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