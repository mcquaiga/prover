namespace Core.Enums
{
    public enum DriveTypeDescripter
    {
        Mechanical,
        Rotary
    }

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

    public enum TestLevel
    {
        Level1 = 0,
        Level2 = 1,
        Level3 = 2
    }
}