using System.Diagnostics.CodeAnalysis;

namespace Prover.Shared
{
    public enum PulseOutputChannel
    {
        Channel_A,
        Channel_B,
        Channel_C
    }

    public enum PulseOutputType
    {
        CorVol,
        PCorVol,
        UncVol,
        NoOut,
        Time
    }

    public enum CompositionType
    {
        T,
        P,
        PTZ,
        Fixed
    }

    public enum RotaryMeterMountType
    {
        B3,
        LMMA,
        RM
    }

    public enum CorrectionFactorType
    {
        Fixed = 1,
        Live = 0
    }

    public enum VolumeInputType
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



    public enum TemperatureUnitType
    {
        C,

        F,

        R,

        K
    }
}