using Devices.Core.Items.ItemGroups;
using Newtonsoft.Json;
using Prover.Calculations;

namespace Prover.Application.Models.EvcVerifications.Verifications.Volume
{
    public class CorrectedVolumeTestRun : VerificationTestEntity<VolumeItems, VolumeItems>, IPulseOutputVerification
    {
        [JsonConstructor]
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
            UncorrectedInputVolume = uncorrectedInputVolume;
            Calculate(tempFactor, pressureFactor, superFactorSquared);
        }

        public CorrectedVolumeTestRun()
        {
        }

        public decimal TotalCorrectionFactor { get; set; }

        public decimal UncorrectedInputVolume { get; set; }

        public decimal PassTolerance => Tolerances.COR_ERROR_THRESHOLD;
        public PulseOutputVerification PulseOutputTest { get; set; }

        public void Calculate(decimal? tempFactor, decimal? pressureFactor, decimal? superFactorSquared)
        {
            TotalCorrectionFactor = Calculators.TotalCorrectionFactor(tempFactor, pressureFactor, superFactorSquared);
            ActualValue = VolumeCalculator.TotalVolume(StartValues.CorrectedReading, EndValues.CorrectedReading, StartValues.CorrectedMultiplier);
            ExpectedValue = VolumeCalculator.TrueCorrected(TotalCorrectionFactor, UncorrectedInputVolume);
            PercentError = Calculators.PercentDeviation(ExpectedValue, ActualValue);
        }

        //public void SetUncorrectedInputVolume(VolumeInputType driveInputType, decimal appliedInput)
        //{
        //    UncorrectedInputVolume = driveInputType.UnCorrectedInputVolume(appliedInput);
        //}
    }
}