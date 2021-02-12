using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Shared.Interfaces;
using RJCP.IO.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Prover.Application.Hardware {
	public interface IAppliedInputVolume : IDisposable {
		/// <summary>
		///     The ReadTach
		/// </summary>
		/// <returns>The <see cref="System.Threading.Tasks.Task" /></returns>
		Task<int> GetAppliedInput();

		/// <summary>
		///     The ResetTach
		/// </summary>
		/// <returns>The <see cref="System.Threading.Tasks.Task{TResult}" /></returns>
		Task ResetAppliedInput();
	}

	public class NullTachometerService : IAppliedInputVolume {
		public void Dispose() {

		}

		public async Task<int> GetAppliedInput() {
			await Task.CompletedTask;
			return 0;
		}

		public async Task ResetAppliedInput() {
			await Task.CompletedTask;
		}


	}

	/// <summary>
	///     Defines the <see cref="TachometerService" />
	/// </summary>
	public class TachometerService : IAppliedInputVolume {
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
		public TachometerService(ILogger logger, string portName, IOutputChannel outputBoard) {
			Logger = logger ?? NullLogger.Instance;

			if (string.IsNullOrEmpty(portName) || !SerialPortStream.GetPortNames().Contains(portName))
				throw new ArgumentException($"Could not find serial port {portName} on this computer.");

			_serialPort = new SerialPortStream(portName, 9600);

			_outputBoard = outputBoard;
		}

		/// <summary>
		///     The Dispose
		/// </summary>
		public void Dispose() {
			_serialPort?.Close();
			_serialPort?.Dispose();
			(_outputBoard as IDisposable)?.Dispose();
		}

		/// <summary>
		///     The ReadTach
		/// </summary>
		/// <returns>The <see cref="System.Threading.Tasks.Task" /></returns>
		public async Task<int> GetAppliedInput() {
			if (_serialPort == null)
				return -1;


			if (!_serialPort.IsOpen)
				_serialPort.Open();

			_serialPort.DiscardInBuffer();
			_serialPort.Write("@D0");
			_serialPort.Write(((char)13).ToString());

			await Task.Delay(100);

			var tachString = _serialPort.ReadExisting();
			Logger.LogDebug($"Read data from Tach: {tachString}");

			return ParseTachValue(tachString);
		}

		/// <summary>
		///     The ResetTach
		/// </summary>
		/// <returns>The <see cref="System.Threading.Tasks.Task{TResult}" /></returns>
		public async Task ResetAppliedInput() {
			var tasks = new List<Task>();
			tasks.Add(Task.Run(() => {
				if (_serialPort == null)
					return;

				if (!_serialPort.IsOpen)
					_serialPort.Open();

				_serialPort.Write($"@T1{(char)13}");
				Thread.Sleep(50);
				_serialPort.Write($"6{(char)13}");
				_serialPort.DiscardInBuffer();
			}));

			tasks.Add(Task.Run(() => {
				_outputBoard?.SignalStart();
				Thread.Sleep(500);
				_outputBoard?.SignalStop();
				Thread.Sleep(100);
			}));

			Task.WaitAll(tasks.ToArray());

			if (await GetAppliedInput() > 0) {
				await ResetAppliedInput();
			}
		}


		/// <summary>
		///     The ParseTachValue
		/// </summary>
		/// <param name="value">The value<see cref="string" /></param>
		/// <returns>The <see cref="int" /></returns>
		private static int ParseTachValue(string value) {
			if (string.IsNullOrEmpty(value))
				return 0;

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