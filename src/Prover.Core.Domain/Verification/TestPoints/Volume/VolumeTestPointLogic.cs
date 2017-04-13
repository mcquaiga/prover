using Prover.Domain.Instrument.Items;

namespace Prover.Domain.Verification.TestPoints.Volume
{
    public partial class VolumeTestPoint
    {
        private CorrectedVolumeCalculator _correctedCalculator;
        private UncorrectedVolumeCalculator _uncorrectedCalculator;
        private double? _superFactor;
        private double? _pressureFactor;
        private double? _temperatureFactor;

        public IVolumeCalculator CorrectedCalculator => _correctedCalculator;
        public IVolumeCalculator UncorrectedCalculator => _uncorrectedCalculator;

        public void Start(IVolumeItems startTestVolumeItems)
        {
            PreTestItems = startTestVolumeItems;
            PostTestItems = null;

            _uncorrectedCalculator = null;
            _correctedCalculator = null;
            AppliedInput = 0;
        }

        public void Finish(IVolumeItems endTestVolumeItems, double appliedInput,
            double? temperatureFactor, double? pressureFactor, double? superFactor)
        {
            PostTestItems = endTestVolumeItems;
            Update(appliedInput, temperatureFactor, pressureFactor, superFactor);
        }

        internal void Calculate(double? temperatureFactor, double? pressureFactor, double? superFactor)
        {
            _temperatureFactor = temperatureFactor;
            _pressureFactor = pressureFactor;
            _superFactor = superFactor;

            _uncorrectedCalculator = new UncorrectedVolumeCalculator(DriveType,
                EvcItems.UncorrectedMultiplier,
                AppliedInput,
                PreTestItems.UncorrectedReading,
                PostTestItems.UncorrectedReading);

            _correctedCalculator = new CorrectedVolumeCalculator(EvcItems.CorrectedMultiplier,
                _uncorrectedCalculator.Calculated,
                PreTestItems.CorrectedReading, PostTestItems.CorrectedReading,
                temperatureFactor, pressureFactor, superFactor);
        }

        private void Update(IVolumeItems preTestItems, IVolumeItems postTestItems, double? appliedInput,
            double? temperatureFactor, double? pressureFactor, double? superFactor)
        {
            PreTestItems = preTestItems;
            PostTestItems = postTestItems;

            Update(appliedInput ?? AppliedInput, temperatureFactor, pressureFactor, superFactor);
        }

        private void Update(double appliedInput, double? temperatureFactor, double? pressureFactor, double? superFactor)
        {
            if (PreTestItems == null || PostTestItems == null) return;

            AppliedInput = appliedInput;
            Calculate(temperatureFactor, pressureFactor, superFactor);
        }
    }
}