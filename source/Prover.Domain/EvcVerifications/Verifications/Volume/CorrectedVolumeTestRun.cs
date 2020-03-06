using Core.GasCalculations;
using Devices.Core.Items.ItemGroups;
using Prover.Domain.EvcVerifications.Verifications.Volume.InputTypes;

namespace Prover.Domain.EvcVerifications.Verifications.Volume
{


    public class CorrectedVolumeTestRun : VerificationTestEntity<VolumeItems, VolumeItems>, IPulseOutputVerification
    {
        private CorrectedVolumeTestRun()
        {
        }

        public CorrectedVolumeTestRun(VolumeItems startValues, VolumeItems endValues, decimal expectedValue,
            decimal actualValue, decimal percentError, bool verified, decimal totalCorrectionFactor,
            decimal uncorrectedInputVolume)
            : base(startValues, endValues, expectedValue, actualValue, percentError, verified)
        {
            TotalCorrectionFactor = totalCorrectionFactor;
            UncorrectedInputVolume = uncorrectedInputVolume;
        }

        public decimal TotalCorrectionFactor { get; set; }

        public decimal UncorrectedInputVolume { get; set; }

        public decimal PassTolerance => Global.COR_ERROR_THRESHOLD;
        public PulseOutputVerification PulseOutputTest { get; set; }

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
    }
}