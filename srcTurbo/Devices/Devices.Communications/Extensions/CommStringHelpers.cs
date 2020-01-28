namespace Devices.Communications.Extensions
{
    public static class CommStringHelpers
    {
        #region Public Methods

        public static string ScrubInvalidCharacters(this string value)
        {
            var result = value;
            foreach (var c in InvalidCharacters)
            {
                result = result.Replace(c, string.Empty);
            }

            return result;
        }

        #endregion Public Methods

        #region Private Fields

        private static readonly string[] InvalidCharacters = { "\"" };

        #endregion Private Fields
    }
}