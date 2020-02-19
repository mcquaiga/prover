using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Application
{
    internal static class Keys
    {
        public const string AppDataSettingsKey  = "Global:AppData";
    }

    public static class AppDefaults
    {
        public static string DataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "EvcProver");

        public class Settings
        {
            public static string FilePath = Path.Combine(DataDirectory, "settings.conf");
        }

        public static void SetValues(IConfiguration config)
        {
            AppDefaults.DataDirectory = FindValue(config, Keys.AppDataSettingsKey);
        }

        private static string FindValue(IConfiguration config, string key)
        {
            var path = config.GetValue<string>(key);
            if (!string.IsNullOrEmpty(path))
            {
                 path = Environment.ExpandEnvironmentVariables(path);
            }

            return path;
        }
    }
}
