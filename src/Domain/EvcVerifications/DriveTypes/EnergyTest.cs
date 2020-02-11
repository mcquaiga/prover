using Core.GasCalculations;
using Devices.Core;
using Devices.Core.Items.DriveTypes;
using Domain.EvcVerifications.CorrectionTests;

namespace Domain.EvcVerifications.DriveTypes
{
    public class EnergyTest : CorrectionTest
    {
        public EnergyTest(EnergyUnitType energyUnitType, decimal startEnergyReading, decimal endEnergyReading,
            decimal evcCorrected,
            decimal energyGasValue) : base(CorrectionFactorTestType.Energy)
        {
            ActualValue = endEnergyReading - startEnergyReading;
            ExpectedValue = EnergyCalculator.Calculated(energyUnitType, evcCorrected, energyGasValue);
        }

        #region Public Methods

        public static EnergyTest Create(IEnergyItems startValues, IEnergyItems endValues, decimal? evcCorrected)
        {
            return new EnergyTest(startValues.EnergyUnitType, startValues.EnergyReading, endValues.EnergyReading,
                evcCorrected ?? 0, endValues.EnergyGasValue);
        }

        #endregion
    }
}