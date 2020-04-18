using Devices.Core.Items.DriveTypes;

namespace Prover.Application.Models.EvcVerifications.Verifications.Volume
{
    public class EnergyTest : VerificationTestEntity<EnergyItems, EnergyItems>
    {
        //public EnergyTest(IEnergyItems startValues, IEnergyItems endValues, decimal evcCorrected, decimal energyGasValue) 
        //    : base(VerificationTestType.Energy, startValues, endValues)
        //{
        //    ActualValue = endValues.EnergyReading - startValues.EnergyReading;
        //    ExpectedValue = EnergyCalculator.Calculated(startValues.EnergyUnitType, evcCorrected, energyGasValue);
        //}

        #region Public Methods

        public EnergyTest(EnergyItems startValues, EnergyItems endValues, decimal expectedValue, decimal actualValue, decimal percentError, bool verified) 
            : base(startValues, endValues, expectedValue, actualValue, percentError, verified)
        {
        }
        public EnergyTest(){}
        //public static EnergyTest Create(IEnergyItems startValues, IEnergyItems endValues, decimal? evcCorrected)
        //{
        //    return new EnergyTest(startValues, endValues, evcCorrected ?? 0, endValues.EnergyGasValue);
        //}

        #endregion
    }
}