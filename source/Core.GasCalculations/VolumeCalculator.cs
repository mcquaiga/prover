using Prover.Shared.Extensions;

namespace Core.GasCalculations
{
    public class VolumeCalculator
    {
        public static decimal TrueCorrected(decimal totalCorrectionFactor, decimal uncorrectedVolume)
        {
            return Round.Volume(totalCorrectionFactor * uncorrectedVolume);
        }

        public static decimal TrueUncorrected(decimal rate, decimal appliedInput)
        {
            return Round.Volume(rate * appliedInput);
        }

        public static decimal TotalVolume(decimal startReading, decimal endReading, decimal multiplier)
        {
            return Round.Volume((endReading - startReading) * multiplier);
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