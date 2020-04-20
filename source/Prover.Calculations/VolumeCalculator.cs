using Prover.Shared.Extensions;

namespace Prover.Calculations
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
            return Round.Volume(TotalVolume(startReading, endReading) * multiplier);
        }
        
        public static decimal TotalVolume(decimal startReading, decimal endReading)
        {
            return Round.Volume(endReading - startReading);
        }

        public static int PulseCount(decimal totalVolume, decimal? multiplier)
        {
            return (int)(decimal.ToInt32(totalVolume) / (multiplier ?? 1));
        }
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