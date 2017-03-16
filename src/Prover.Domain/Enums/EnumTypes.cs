using System.Diagnostics.CodeAnalysis;

namespace Prover.Shared.Enums
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum EvcCorrectorType
    {
        T,
        P,
        PTZ
    }

    public enum TestLevel
    {
        Level1 = 0,
        Level2 = 1,
        Level3 = 2
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum TemperatureUnits
    {
        C,
        F,
        R,
        K
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
}