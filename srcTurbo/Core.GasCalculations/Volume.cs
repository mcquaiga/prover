using Core.GasCalculations.Helpers;
using Devices.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.GasCalculations
{
    public static class Energy
    {
        public static decimal Calculated(EnergyUnits units, decimal totalCorrectedVolume, decimal totalEnergyValue)
        {
            decimal result;
            switch (units)
            {
                case EnergyUnits.Therms:
                    result = totalCorrectedVolume * totalEnergyValue / 100000;
                    break;

                case EnergyUnits.Dktherms:
                    result = totalCorrectedVolume * totalEnergyValue / 1000000;
                    break;

                case EnergyUnits.GigaJoules:
                    result = totalCorrectedVolume * 0.028317m * totalEnergyValue / 1000000;
                    break;

                case EnergyUnits.MegaJoules:
                    result = totalCorrectedVolume * 0.028317m * totalEnergyValue / 1000;
                    break;

                case EnergyUnits.KiloCals:
                    result = totalCorrectedVolume * 0.0283168m * totalEnergyValue;
                    break;

                default:
                    throw new Exception(string.Format("Energy units not supported: {0}", units));
            }

            return Round.Factor(result);
        }
    }

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