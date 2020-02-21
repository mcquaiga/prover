using Core.GasCalculations;
using Devices.Core.Items.ItemGroups;
using Domain.EvcVerifications.Verifications.Volume.InputTypes;

namespace Domain.EvcVerifications.Verifications.Volume
{
    public class CorrectedVolumeTestRun : VerificationTestEntity<VolumeItems, VolumeItems>
    {
        private CorrectedVolumeTestRun() : base()
        {

        }

        #region Public Properties

        public CorrectedVolumeTestRun(VolumeItems startValues, VolumeItems endValues, decimal expectedValue, decimal actualValue, decimal percentError, bool verified, decimal totalCorrectionFactor, decimal uncorrectedInputVolume) 
            : base(startValues, endValues, expectedValue, actualValue, percentError, verified)
        {
            TotalCorrectionFactor = totalCorrectionFactor;
            UncorrectedInputVolume = uncorrectedInputVolume;
        }

        public decimal TotalCorrectionFactor { get; set; }

        public decimal UncorrectedInputVolume { get; set; }

        public decimal PassTolerance => Global.COR_ERROR_THRESHOLD;


        public void Calculate()
        {
            ActualValue = VolumeCalculator.TotalVolume(StartValues.CorrectedReading, EndValues.CorrectedReading);
            ExpectedValue = VolumeCalculator.TrueCorrected(TotalCorrectionFactor, UncorrectedInputVolume);

            PercentError = Calculators.PercentDeviation(ExpectedValue, ActualValue);
        }

        public void SetTotalCorrectionFactor(decimal? tempFactor, decimal? pressureFactor, decimal? superFactorSquared)
        {
            TotalCorrectionFactor = Calculators.TotalCorrectionFactor(tempFactor, pressureFactor, superFactorSquared);
        }

        public void SetUncorrectedInputVolume(IVolumeInputType driveInputType, decimal appliedInput)
        {
            UncorrectedInputVolume = driveInputType.UnCorrectedInputVolume(appliedInput);
        }
        #endregion
    }
}