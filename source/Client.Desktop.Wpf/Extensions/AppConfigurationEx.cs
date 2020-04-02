using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Client.Desktop.Wpf.Extensions
{
    public static class AppConfigurationEx
    {
        private const string LiteDbKey = "Storage:LiteDb";
        private const string AppDataKey = "AppDataDir";

        private const string DefaultAppData = ".\\";

        public static string AppDataPath(this IConfiguration config)
        {
            var path = Environment.ExpandEnvironmentVariables(config.GetValue<string>(AppDataKey));

            return !string.IsNullOrEmpty(path) ? path : DefaultAppData;
        }

        public static string LiteDbPath(this IConfiguration config)
        {
            return Path.Combine(config.AppDataPath(), (config.GetValue<string>(LiteDbKey)));
        }

        public static bool IsLiteDb(this IConfiguration config)
        {
            return !string.IsNullOrEmpty(config.GetValue<string>(LiteDbKey));
        }
    }
}