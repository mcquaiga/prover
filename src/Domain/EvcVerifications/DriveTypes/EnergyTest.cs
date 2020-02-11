using Core.GasCalculations;
using Devices.Core;
using Devices.Core.Items.DriveTypes;
using Domain.EvcVerifications.CorrectionTests;

namespace Domain.EvcVerifications.DriveTypes
{
    public class EnergyTest : TestRunBase<IEnergyItems>
    {
        private EnergyTest(EnergyUnitType energyUnitType, decimal startEnergyReading, decimal endEnergyReading,
            decimal evcCorrected,
            decimal energyGasValue)
        {
            Actual = endEnergyReading - startEnergyReading;
            Expected = EnergyCalculator.Calculated(energyUnitType, evcCorrected, energyGasValue);
        }

        #region Public Methods

        public static EnergyTest Create(IEnergyItems startValues, IEnergyItems endValues, decimal? evcCorrected)
        {
            return new EnergyTest(startValues.EnergyUnitType, startValues.EnergyReading, endValues.EnergyReading,
                evcCorrected ?? 0, endValues.EnergyGasValue);
        }

        #endregion
    }

    //public class MechanicalDrive : IDriveType
    //{
    //    public string Discriminator => Drives.Mechanical;
    //    public Energy Energy { get; set; }

    //    public MechanicalDrive(IEnergyItems energyItems)
    //    {
    //        Energy = new Energy(energyItems);
    //    }

    //    public int MaxUncorrectedPulses()
    //    {
    //        return _mechanicalUncorrectedTestLimits?
    //                   .FirstOrDefault(x => x.CuFtValue == _uncorUnits)?.UncorrectedPulses ?? 10;
    //    }

    //    public decimal UnCorrectedInputVolume(decimal appliedInput)
    //    {
    //        return appliedInput * Volume.DriveRate;
    //    }

    //    private readonly List<CorrectionTestDefinition.MechanicalUncorrectedTestLimit> _mechanicalUncorrectedTestLimits;
    //}
}