using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Prover.Application
{
    internal static class Keys
    {
        public const string AppDataSettingsKey  = "Global:AppData";
    }

    public static class AppDefaults
    {
        public static string AppDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EvcProver");

        public class Settings
        {
            public static string FilePath = Path.Combine(AppDataDirectory, "settings.conf");
        }

        //public static void SetValues(IConfiguration config)
        //{
        //    AppDefaults.DataDirectory = FindValue(config, Keys.AppDataSettingsKey);
        //}

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

    public static class Tolerances
    {
        /// <summary>
        /// Defines the COR_ERROR_THRESHOLD
        /// </summary>
        public const decimal COR_ERROR_THRESHOLD = 1.5M;

        /// <summary>
        /// Defines the METER_DIS_ERROR_THRESHOLD
        /// </summary>
        public const decimal METER_DIS_ERROR_THRESHOLD = 1;

        /// <summary>
        /// Defines the PRESSURE_ERROR_TOLERANCE
        /// </summary>
        public const decimal PRESSURE_ERROR_TOLERANCE = 1.0M;

        /// <summary>
        /// Defines the PULSE_VARIANCE_THRESHOLD
        /// </summary>
        public const int PULSE_VARIANCE_THRESHOLD = 2;

        /// <summary>
        /// Defines the SUPER_FACTOR_TOLERANCE
        /// </summary>
        public const decimal SUPER_FACTOR_TOLERANCE = 0.3M;

        /// <summary>
        /// Defines the TEMP_ERROR_TOLERANCE
        /// </summary>
        public const decimal TEMP_ERROR_TOLERANCE = 1.0M;

        /// <summary>
        /// Defines the UNCOR_ERROR_THRESHOLD
        /// </summary>
        public const decimal UNCOR_ERROR_THRESHOLD = 0.1M;

        public static decimal ENERGY_PASS_TOLERANCE = 1.0m;
    }
}
