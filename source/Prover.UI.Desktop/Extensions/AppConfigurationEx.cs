using Microsoft.Extensions.Configuration;
using System;

namespace Prover.UI.Desktop.Extensions
{
    public static class AppConfig
    {

    }

    public static class AppConfigurationEx
    {
        private const string LiteDbKey = "Storage:LiteDb";
        private const string UseAzureKey = "Storage:UseAzure";
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

        public static bool UseLiteDb(this IConfiguration config) => !string.IsNullOrEmpty(config.GetValue<string>(LiteDbKey));
        public static bool UseAzure(this IConfiguration config) => config.GetValue<bool?>(UseAzureKey) ?? false;

        public static string GetValueExpanded(this IConfiguration config, string key)
        {
            return Environment.ExpandEnvironmentVariables(config.GetValue<string>(key));
        }
    }
}