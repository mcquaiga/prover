﻿using System;
using Core.GasCalculations.Helpers;
using Devices.Core;
using Devices.Core.Items.DriveTypes;

namespace Core.GasCalculations
{
    public class EnergyCalculator
    {
        public IEnergyItems StartEnergyItems { get; }
        public IEnergyItems EndEnergyItems { get; }
    
        public EnergyCalculator(IEnergyItems startEnergyItems, IEnergyItems endEnergyItems)
        {
            StartEnergyItems = startEnergyItems;
            EndEnergyItems = endEnergyItems;
        }

        public decimal EnergyValueCalculated()
        {
            return EndEnergyItems.EnergyGasValue - StartEnergyItems.EnergyGasValue;
        }

        public decimal Calculated(decimal totalCorrectedVolume, decimal totalEnergyValue)
        {
            decimal result;
            switch (StartEnergyItems.EnergyUnitType)
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
                    throw new Exception($"Energy units not supported: {units}");
            }

            return Round.Factor(result);

        }
    }
}