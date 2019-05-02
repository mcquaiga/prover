namespace Devices.Core
{
    public enum CompositionType
    {
        TemperatureOnly,

        PressureOnly,

        PressureTemperature
    }

    public enum DriveTypeDescripter
    {
        Mechanical,

        Rotary
    }

    public enum PressureTransducerType
    {
        Absolute,

        Gauge
    }

    public enum PressureUnits
    {
        PSIA,

        PSIG,

        kPa,

        mPa,

        BAR,

        mBAR,

        KGcm2,

        inWC,

        inHG,

        mmHG
    }

    public enum TemperatureUnits
    {
        C,

        F,

        R,

        K
    }
}