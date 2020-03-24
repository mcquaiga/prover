namespace Prover.CommProtocol.Common.Extensions
{
    public static class CommStringHelpers
    {
        private static readonly string[] InvalidCharacters = { "\"" };

        public static string ScrubInvalidCharacters(this string value)
        {
            var result = value;
            foreach (var c in InvalidCharacters)
            {
                result = result.Replace(c, string.Empty);
            }

            return result;
        }
    }
}
