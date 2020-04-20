using System;
using Microsoft.Extensions.Configuration;

namespace Prover.UI.Desktop.Extensions
{
    public static class AppConfigurationEx
    {
        private const string LiteDbKey = "Storage:LiteDb";
        private const string AppDataKey = "AppDataDir";

        private const string DefaultAppData = ".\\";

        public static string AppDataPath(this IConfiguration config)
        {
            var path = config.GetValueExpanded(AppDataKey);

            return !string.IsNullOrEmpty(path) ? path : DefaultAppData;
        }

        public static string LiteDbPath(this IConfiguration config)
        {
            return config.GetValueExpanded(LiteDbKey);
            //return Path.Combine(config.AppDataPath(), (config.GetValue<string>(LiteDbKey)));
        }

        public static bool IsLiteDb(this IConfiguration config)
        {
            return !string.IsNullOrEmpty(config.GetValue<string>(LiteDbKey));
        }

        public static string GetValueExpanded(this IConfiguration config, string key)
        {
            return Environment.ExpandEnvironmentVariables(config.GetValue<string>(key));
        }
    }
}