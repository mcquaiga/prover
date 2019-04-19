#region

using System.Diagnostics.CodeAnalysis;

#endregion

namespace Prover.CommProtocol.Common
{
    public enum DriveTypeDescripter
    {
        Mechanical,
        Rotary
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum EvcCorrectorType
    {
        T,
        P,
        PTZ
    }

    public enum PressureTransducerType
    {
        Absolute,
        Gauge
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
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

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum TemperatureUnits
    {
        C,
        F,
        R,
        K
    }

    public enum TestLevel
    {
        Level1 = 0,
        Level2 = 1,
        Level3 = 2
    }
}