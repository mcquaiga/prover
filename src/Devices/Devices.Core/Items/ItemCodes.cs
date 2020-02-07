namespace Devices.Core.Items
{
    public static class ItemCodes
    {
        #region Public Classes

        public static class Pressure
        {
            #region Public Fields

            public const string Atm = "ATM_PRESS";
            public const string Base = "BASE_PRESS";
            public const string Factor = "PRESS_FACTOR";
            public const string FixedFactor = "FIXED_PRESS_FACTOR";
            public const string GasPressure = "GAS_PRESS";
            public const string Range = "PRESS_RANGE";
            public const string TransducerType = "TRANSDUCER_TYPE";
            public const string Units = "PRESS_UNITS";
            public const string UnsqrFactor = "UNSQRD_SUPER_FACTOR";

            #endregion Public Fields
        }

        public static class SiteInfo
        {
            #region Public Fields

            public const string CompanyNumber = "SITENUMBER2";
            public const string Firmware = "FIRMWARE";
            public const string SerialNumber = "SERIAL_NUM";

            #endregion Public Fields
        }

        public static class Super
        {
            #region Public Fields

            public static string FixedFactor = "FIXED_SUPER_FACTOR";

            #endregion Public Fields

            //private int CO2_NUMBER = 55;
            //private int N2_NUMBER = 54;
            //private int SPEC_GR_NUMBER = 53;
            //private int SUPER_TABLE_NUMBER = 147;
        }

        public static class Temperature
        {
            #region Public Fields

            public static string Factor = "TEMP_FACTOR";
            public static string FixedFactor = "FIXED_TEMP_FACTOR";
            public static string GasTemperature = "GAS_TEMP";

            #endregion Public Fields
        }

        #endregion Public Classes
    }
}