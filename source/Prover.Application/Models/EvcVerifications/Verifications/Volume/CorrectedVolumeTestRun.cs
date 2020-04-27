using Devices.Core.Items.ItemGroups;
using Prover.Application.Models.EvcVerifications.Verifications.Volume.InputTypes;
using Prover.Calculations;

namespace Prover.Application.Models.EvcVerifications.Verifications.Volume
{
    public class CorrectedVolumeTestRun : VerificationTestEntity<VolumeItems, VolumeItems>, IPulseOutputVerification
    {
        public CorrectedVolumeTestRun
        (VolumeItems startValues, VolumeItems endValues, decimal expectedValue, decimal actualValue, decimal percentError, bool verified, decimal totalCorrectionFactor,
                decimal uncorrectedInputVolume
        ) : base(startValues, endValues, expectedValue, actualValue, percentError, verified)
        {
            TotalCorrectionFactor = totalCorrectionFactor;
            UncorrectedInputVolume = uncorrectedInputVolume;
        }

        public CorrectedVolumeTestRun
                (VolumeItems startValues, VolumeItems endValues, decimal uncorrectedInputVolume, decimal? tempFactor, decimal? pressureFactor, decimal? superFactorSquared)
                : base(startValues, endValues, 0m, 0m, 100m, false)
        {
            SetTotalCorrectionFactor(tempFactor, pressureFactor, superFactorSquared);
            UncorrectedInputVolume = uncorrectedInputVolume;
            Calculate();
        }

        public CorrectedVolumeTestRun()
        {
        }

        public decimal TotalCorrectionFactor { get; set; }

        public decimal UncorrectedInputVolume { get; set; }

        public decimal PassTolerance => Tolerances.COR_ERROR_THRESHOLD;
        public PulseOutputVerification PulseOutputTest { get; set; }

        public void Calculate()
        {
            ActualValue = VolumeCalculator.TotalVolume(StartValues.CorrectedReading, EndValues.CorrectedReading, StartValues.CorrectedMultiplier);
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
    }
}