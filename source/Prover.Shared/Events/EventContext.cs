using System;
using System.Threading;

namespace Prover.Shared.Events
{
	public sealed class EventContext<TInput, TOutput>
	{
		private readonly TInput _input;
		private TOutput _output;
		private int _outputSet;

		internal EventContext(TInput input)
		{
			_input = input;
		}

		/// <summary>
		/// Gets the input for the interaction.
		/// </summary>
		public TInput Input => _input;

		/// <summary>
		/// Gets a value indicating whether the interaction is handled. That is, whether the output has been set.
		/// </summary>
		public bool IsHandled => _outputSet == 1;

		/// <summary>
		/// Sets the output for the interaction.
		/// </summary>
		/// <param name="output">
		/// The output.
		/// </param>
		/// <exception cref="InvalidOperationException">
		/// If the output has already been set.
		/// </exception>
		public void SetOutput(TOutput output)
		{
			if (Interlocked.CompareExchange(ref _outputSet, 1, 0) != 0)
			{
				throw new InvalidOperationException("Output has already been set.");
			}

			_output = output;
		}

		/// <summary>
		/// Gets the output of the interaction.
		/// </summary>
		/// <returns>
		/// The output.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// If the output has not been set.
		/// </exception>
		public TOutput GetOutput()
		{
			if (_outputSet == 0)
			{
				throw new InvalidOperationException("Output has not been set.");
			}

			return _output;
		}
	}
}