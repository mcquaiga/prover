using Core.GasCalculations.Helpers;
using System.Collections.Generic;
using System.Text;

namespace Core.GasCalculations
{
    public static class Volume
    {
        public static decimal CorrectedVolume(decimal totalCorrectionFactor, decimal uncorrectedVolume)
        {
            return Round.Factor(totalCorrectionFactor * uncorrectedVolume);
        }

        public static decimal UncorrectedVolume(decimal driveRate, decimal appliedInput)
        {
            return Round.Factor(driveRate * appliedInput);
        }

        public static class Mechanical
        {
            public static decimal UncorrectedVolume(decimal driveRate, decimal appliedInput)
            {
                return Round.Factor(driveRate * appliedInput);
            }
        }

        public static class Rotary
        {
            public static decimal UncorrectedVolume(decimal meterDisplacement, decimal appliedInput)
            {
                return Round.Factor(meterDisplacement * appliedInput);
            }
        }
    }
}