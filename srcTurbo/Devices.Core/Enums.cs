namespace Devices.Core
{
    public enum CompositionType
    {
        TemperatureOnly,
        PressureOnly,
        PressureTemperature
    }

    public enum CorrectionFactorType
    {
        Fixed,
        Live
    }

    public enum DriveType
    {
        Mechanical,
        PulseInput,
        Rotary
    }

    public enum EnergyUnitType
    {
        Therms,
        DkTherms,
        MegaJoules,
        GigaJoules,
        KiloCals,
        KiloWattHours
    }

    public enum PressureTransducerType
    {
        Absolute,

        Gauge
    }

    public enum PressureUnitType
    {
        Psia,
        Psig,
        kPa,
        mPa,
        BAR,
        mBAR,
        KGcm2,
        inWC,
        inHG,
        mmHG
    }

    public enum PulseOutputUnitType
    {
        CorVol,
        PCorVol,
        UncVol,
        NoOut,
        Time
    }

    public enum TemperatureUnitType
    {
        C,

        F,

        R,

        K
    }
}