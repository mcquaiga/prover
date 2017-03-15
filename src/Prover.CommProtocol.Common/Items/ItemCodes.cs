namespace Prover.CommProtocol.Common.Items
{
    public enum ItemName
    {
        //Site Information
        SerialNumber,
        InventoryNumber,
        FirmwareVersion,

        //Pressure
        PressureFactorFixed,
        PressureBase,
        PressureGas,
        PressureAtm,
        PressureTransducerType,
        PressureRange,

        //Temperature
        TemperatureFactorFixed,

        //SuperFactor
        SuperFactorFixed
    }

    public static class ItemCodes
    {
        public static class SiteInfo
        {
            public const string SerialNumber = "SERIAL_NUM";
            public const string Firmware = "FIRMWARE";
            public const string CompanyNumber = "SITENUMBER2";
        }

        public static class Pressure
        {
            public const string Base = "BASE_PRESS";
            public const string Atm = "ATM_PRESS";
            public const string Units = "PRESS_UNITS";
            public const string Range = "PRESS_RANGE";
            public const string TransducerType = "TRANSDUCER_TYPE";
            public const string GasPressure = "GAS_PRESS";
            public const string Factor = "PRESS_FACTOR";
            public const string UnsqrFactor = "UNSQRD_SUPER_FACTOR";
            public const string FixedFactor = "FIXED_PRESS_FACTOR";
        }

        public static class Temperature
        {
            public static string FixedFactor = "FIXED_TEMP_FACTOR";
            public static string Factor = "TEMP_FACTOR";
            public static string GasTemperature = "GAS_TEMP";
        }

        public static class Super
        {
            public static string FixedFactor = "FIXED_SUPER_FACTOR";
            //private int CO2_NUMBER = 55;
            //private int N2_NUMBER = 54;
            //private int SPEC_GR_NUMBER = 53;
            //private int SUPER_TABLE_NUMBER = 147;
        }
    }
}