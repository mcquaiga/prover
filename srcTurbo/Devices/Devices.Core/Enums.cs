using System.Diagnostics.CodeAnalysis;

namespace Devices.Core
{
    public enum CompositionType
    {
        T,
        P,
        PTZ,
        Fixed
    }

    public enum CorrectionFactorType
    {
        Fixed = 1,
        Live = 0
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
        DecaTherms,
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

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum PressureUnitType
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