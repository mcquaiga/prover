﻿using System;
using Prover.Shared;
using Prover.Shared.Extensions;

namespace Core.GasCalculations
{
    public class EnergyCalculator
    {

        public decimal EnergyValueCalculated()
        {
            return 0m; //EndEnergyItems.EnergyGasValue - StartEnergyItems.EnergyGasValue;
        }

        public static decimal Calculated(EnergyUnitType energyUnits, decimal totalCorrectedVolume, decimal totalEnergyValue)
        {
            decimal result;
            switch (energyUnits)
            {
                case EnergyUnitType.Therms:
                    result = (totalCorrectedVolume * totalEnergyValue) / 100000;
                    break;

                case EnergyUnitType.DecaTherms:
                    result = (totalCorrectedVolume * totalEnergyValue) / 1000000;
                    break;

                case EnergyUnitType.GigaJoules:
                    result = (totalCorrectedVolume * 0.028317m * totalEnergyValue) / 1000000;
                    break;

                case EnergyUnitType.MegaJoules:
                    result = (totalCorrectedVolume * 0.028317m * totalEnergyValue) / 1000;
                    break;

                case EnergyUnitType.KiloCals:
                    result = totalCorrectedVolume * 0.0283168m * totalEnergyValue;
                    break;

                default:
                    throw new Exception($"Energy units not supported: {energyUnits}");
            }

            return Round.Factor(result);

        }
    }
}