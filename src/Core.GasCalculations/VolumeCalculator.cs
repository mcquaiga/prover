using Shared.Extensions;

namespace Core.GasCalculations
{
    public class VolumeCalculator
    {
        public static decimal Corrected(decimal totalCorrectionFactor, decimal uncorrectedVolume)
        {
            return Round.Factor(totalCorrectionFactor * uncorrectedVolume);
        }

        public static decimal Uncorrected(decimal rate, decimal appliedInput)
        {
            return Round.Factor(rate * appliedInput);
        }

        //public static class Mechanical
        //{
        //    public static decimal UncorrectedVolume(decimal driveRate, decimal appliedInput)
        //    {
        //        return Round.Factor(driveRate * appliedInput);
        //    }
        //}

        //public static class Rotary
        //{
        //    public static decimal UncorrectedVolume(decimal meterDisplacement, decimal appliedInput)
        //    {
        //        return Round.Factor(meterDisplacement * appliedInput);
        //    }
        //}
    }

    public class MechanicalVolumeCalculator : VolumeCalculator
    {
        public static decimal UncorrectedVolume(decimal driveRate, decimal appliedInput)
        {
            return Round.Factor(driveRate * appliedInput);
        }
    }

    public class RotaryVolumeCalculator : VolumeCalculator
    {
        public static decimal UncorrectedVolume(decimal meterDisplacement, decimal appliedInput)
        {
            return Round.Factor(meterDisplacement * appliedInput);
        }
    }
}