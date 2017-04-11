using Prover.Domain.Verification.TestPoints.Volume.DriveTypes;

namespace Prover.Domain.Verification.TestPoints.Volume
{
    public interface IVolumeCalculator
    {
        double Calculated { get; }
        double EvcTotal { get; }
        double Multiplier { get; }
        double? PercentError { get; }
    }

    internal class CorrectedVolumeCalculator : IVolumeCalculator
    {
        public double Multiplier { get; }
        public double Calculated { get; private set; }
        public double EvcTotal { get; private set; }
        public double? PercentError { get; private set; }

        public CorrectedVolumeCalculator(double correctedMultiplier)
        {
            Multiplier = correctedMultiplier;

            Calculated = 0;
            EvcTotal = 0;
            PercentError = default(double?);
        }

        public CorrectedVolumeCalculator(double correctedMultiplier, double uncorrectedCalculated,
            double startCorrectedReading, double endCorrectedReading, double? temperatureFactor, double? pressureFactor, double? superFactor)
            : this(correctedMultiplier)
        {
            RunCalculations(uncorrectedCalculated, startCorrectedReading, endCorrectedReading, temperatureFactor, pressureFactor, superFactor);
        }

        public void Update(double uncorrectedCalculated, 
            double startCorrectedReading, double endCorrectedReading,
            double? temperatureFactor, double? pressureFactor, double? superFactor)
        {
            RunCalculations(uncorrectedCalculated, startCorrectedReading, endCorrectedReading, temperatureFactor, pressureFactor, superFactor);
        }

        private void RunCalculations(double uncorrectedCalculated, double startCorrectedReading, double endCorrectedReading,
            double? temperatureFactor, double? pressureFactor, double? superFactor)
        {
            var tF = temperatureFactor ?? 1;
            var pF = pressureFactor ?? 1;
            var sF = superFactor ?? 1;

            Calculated = tF * pF * sF * uncorrectedCalculated;
            EvcTotal = (endCorrectedReading - startCorrectedReading) * Multiplier;
            PercentError = (EvcTotal - Calculated) / Calculated * 100;
        }
    }

    internal class UncorrectedVolumeCalculator : IVolumeCalculator
    {
        private readonly IDriveType _driveType;
        public double Calculated { get; private set; }
        public double EvcTotal { get; private set; }
        public double Multiplier { get; }
        public double? PercentError { get; private set; }

        public UncorrectedVolumeCalculator(IDriveType driveType, double uncorrectedMultiplier, 
            double appliedInput, double startUncorrectedReading, double endUncorrectedReading)
        {
            _driveType = driveType;
            Multiplier = uncorrectedMultiplier;

            RunCalculations(appliedInput, startUncorrectedReading, endUncorrectedReading);
        }

        public void Update(double appliedInput, double startUncorrectedReading, double endUncorrectedReading)
        {
            RunCalculations(appliedInput, startUncorrectedReading, endUncorrectedReading);
        }

        private void RunCalculations(double appliedInput, double startUncorrectedReading, double endUncorrectedReading)
        {
            Calculated = _driveType.UnCorrectedInputVolume(appliedInput);
            EvcTotal = (endUncorrectedReading - startUncorrectedReading) * Multiplier;
            PercentError = (EvcTotal - Calculated) / Calculated * 100;
        }
    }
}