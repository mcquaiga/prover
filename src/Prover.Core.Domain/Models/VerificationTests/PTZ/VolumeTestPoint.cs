using Prover.Domain.Models.Instruments;
using Prover.Domain.Models.Instruments.Items;
using Prover.Domain.Models.VerificationTests.DriveTypes;
using Prover.Shared.Enums;

namespace Prover.Domain.Models.VerificationTests.PTZ
{
    public class VolumeTestPoint : IPulseOutputItems
    {
        private readonly PressureTestPoint _pressureTest;
        private readonly SuperFactorTestPoint _superTest;
        private readonly TemperatureTestPoint _temperatureTest;
        private decimal _pressureFactor;
        private decimal _superFactor;
        private decimal _temperatureFactor;

        private VolumeTestPoint(EvcCorrectorType correctorType,
            TemperatureTestPoint temperatureTest,
            PressureTestPoint pressureTest,
            SuperFactorTestPoint superTest)
        {
            CorrectorType = correctorType;

            _temperatureTest = temperatureTest;
            _pressureTest = pressureTest;
            _superTest = superTest;

            TemperatureFactor = 1;
            PressureFactor = 1;
            SuperFactor = 1;

            AppliedInput = 0;
            PulseACount = 0;
            PulseBCount = 0;
            PulseCCount = 0;
        }

        public EvcCorrectorType CorrectorType { get; protected set; }
        public IDriveType DriveType { get; protected set; }
        public decimal AppliedInput { get; set; }

        public IVolumeItems PreTestItems { get; set; }
        public IVolumeItems PostTestItems { get; set; }

        public int PulseACount { get; set; }
        public int PulseBCount { get; set; }
        public int PulseCCount { get; set; }

        /// <summary>
        ///     Temperature factor
        ///     Returns 1 if the Corrector type isn't T or PTZ
        /// </summary>
        public decimal TemperatureFactor
        {
            get
            {
                if (_temperatureTest != null &&
                    (CorrectorType == EvcCorrectorType.T || CorrectorType == EvcCorrectorType.PTZ))
                    return _temperatureTest.ActualFactor;

                return _temperatureFactor;
            }
            set { _temperatureFactor = value; }
        }

        /// <summary>
        /// </summary>
        public decimal PressureFactor
        {
            get
            {
                return CorrectorType == EvcCorrectorType.P || CorrectorType == EvcCorrectorType.PTZ
                    ? _pressureTest?.ActualFactor ?? _pressureFactor
                    : 1.0m;
            }
            set { _pressureFactor = value; }
        }

        public decimal SuperFactor
        {
            get
            {
                return CorrectorType == EvcCorrectorType.PTZ
                    ? _superTest?.ActualFactor ?? _superFactor
                    : 1.0m;
            }
            set { _superFactor = value; }
        }

        /// <summary>
        ///     EVC Corrected volume
        ///     End Corrected - Start Corrected x Corrected Multiplier
        /// </summary>
        public virtual decimal CorrectedEvcTotal
            => (PostTestItems.CorrectedReading - PreTestItems.CorrectedReading) * PreTestItems.CorrectedMultiplier;

        /// <summary>
        ///     Corrected volume variance percent error
        /// </summary>
        public virtual decimal? CorrectedPercentError
            =>
                CorrectedCalculated != 0
                    ? (CorrectedEvcTotal - CorrectedCalculated) / CorrectedCalculated * 100
                    : default(decimal?);

        /// <summary>
        ///     True Corrected volume
        ///     TempFactor x PressFactor x SuperFactor x UncorrectedCalculated
        /// </summary>
        public virtual decimal CorrectedCalculated
            => TemperatureFactor * PressureFactor * SuperFactor * UncorrectedCalculated;

        /// <summary>
        ///     EVC Uncorrected calculated volume
        ///     End Uncorrected - Start Uncorrected x Uncorrected Multiplier
        /// </summary>
        public virtual decimal UncorrectedEvcTotal
            => (PostTestItems.UncorrectedReading - PreTestItems.UncorrectedReading) * PreTestItems.UncorrectedMultiplier
        ;

        /// <summary>
        ///     Uncorrected volume variance percent error
        /// </summary>
        public virtual decimal? UncorrectedPercentError
            =>
                UncorrectedCalculated != 0
                    ? (UncorrectedEvcTotal - UncorrectedCalculated) / UncorrectedCalculated * 100
                    : default(decimal?);

        public virtual decimal UncorrectedCalculated => DriveType.UnCorrectedInputVolume(AppliedInput);

        public virtual bool Passed
            => CorrectedPassed && UncorrectedPassed && DriveType.HasPassed;

        public virtual bool CorrectedPassed
            => CorrectedPercentError < 1 && CorrectedPercentError > -1;

        public virtual bool UncorrectedPassed
            => UncorrectedPercentError < 1 && UncorrectedPercentError > -1;

        public string PulserAUnits { get; }
        public decimal PulserAScaling { get; }
        public string PulserBUnits { get; }
        public decimal PulseBScaling { get; }

        public static VolumeTestPoint Create(IInstrument instrument, TestPoint testPoint)
        {
            var volumeTest = new VolumeTestPoint(instrument.CorrectorType, testPoint.Temperature, testPoint.Pressure,
                testPoint.SuperFactor);

            if (instrument.VolumeItems.DriveType == DriveTypeDescripter.Rotary)
                volumeTest.DriveType = new RotaryDrive(instrument.VolumeItems);
            else
                volumeTest.DriveType = new MechanicalDrive(volumeTest);

            return volumeTest;
        }
    }
}