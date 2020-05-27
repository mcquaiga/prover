using System;

namespace Prover.Shared.Extensions
{
	public static class PropertyExtensions
	{
		public static string Validate(this string input, string paramName = null)
		{
			if (string.IsNullOrEmpty(input))
				throw new ArgumentNullException(paramName ?? nameof(input));

			return input;
		}
	}

	public static class Validate
	{
		public static string NullOrEmpty(string item)
		{
			return item.Validate();
		}
	}
}