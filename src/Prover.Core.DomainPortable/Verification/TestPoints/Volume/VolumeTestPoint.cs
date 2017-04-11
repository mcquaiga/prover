using System;
using Prover.Core.DomainPortable.Instrument;
using Prover.Core.DomainPortable.Instrument.Items;
using Prover.Domain.Verification.TestPoints.Pressure;
using Prover.Domain.Verification.TestPoints.SuperFactor;
using Prover.Domain.Verification.TestPoints.Temperature;
using Prover.Domain.Verification.TestPoints.Volume.DriveTypes;

namespace Prover.Domain.Verification.TestPoints.Volume
{
    public class VolumeTestPoint : Entity<Guid>
    {
        private readonly EvcCorrectorType _correctorType;
        private readonly PressureTestPoint _pressureTest;
        private readonly SuperFactorTestPoint _superTest;
        private readonly TemperatureTestPoint _temperatureTest;

        public VolumeTestPoint(Guid id, EvcCorrectorType correctorType, IVolumeItems preTestItems,
            IVolumeItems postTestItems,
            decimal appliedInput, IDriveType driveType, int pulseACount, int pulseBCount, int pulseCCount,
            TemperatureTestPoint temperatureTest, PressureTestPoint pressureTest, SuperFactorTestPoint superTest)
            : base(id)
        {
            _correctorType = correctorType;
            _temperatureTest = temperatureTest;
            _pressureTest = pressureTest;
            _superTest = superTest;

            PreTestItems = preTestItems;
            PostTestItems = postTestItems;
            AppliedInput = appliedInput;
            DriveType = driveType;
            PulseACount = pulseACount;
            PulseBCount = pulseBCount;
            PulseCCount = pulseCCount;
        }

        private VolumeTestPoint(EvcCorrectorType correctorType, TemperatureTestPoint temperatureTest,
            PressureTestPoint pressureTest, SuperFactorTestPoint superTest)
            : base(Guid.NewGuid())
        {
            _correctorType = correctorType;

            _temperatureTest = temperatureTest;
            _pressureTest = pressureTest;
            _superTest = superTest;

            AppliedInput = 0;
            PulseACount = 0;
            PulseBCount = 0;
            PulseCCount = 0;
        }

        public decimal AppliedInput { get; set; }

        /// <summary>
        ///     True Corrected volume
        ///     TempFactor x PressFactor x SuperFactor x UncorrectedCalculated
        /// </summary>
        public virtual decimal CorrectedCalculated
            => TemperatureFactor * PressureFactor * SuperFactor * UncorrectedCalculated;

        /// <summary>
        ///     EVC Corrected volume
        ///     End Corrected - Start Corrected x Corrected Multiplier
        /// </summary>
        public virtual decimal CorrectedEvcTotal
            => (PostTestItems.CorrectedReading - PreTestItems.CorrectedReading) * PreTestItems.CorrectedMultiplier;

        public virtual bool CorrectedPassed
            => CorrectedPercentError < 1 && CorrectedPercentError > -1;

        /// <summary>
        ///     Corrected volume variance percent error
        /// </summary>
        public virtual decimal? CorrectedPercentError
            =>
                CorrectedCalculated != 0
                    ? (CorrectedEvcTotal - CorrectedCalculated) / CorrectedCalculated * 100
                    : default(decimal?);

        public IDriveType DriveType { get; protected set; }

        public virtual bool Passed
            => CorrectedPassed && UncorrectedPassed && DriveType.HasPassed;

        public IVolumeItems PostTestItems { get; set; }

        /// <summary>
        /// </summary>
        public decimal PressureFactor
        {
            get
            {
                if (_pressureTest != null &&
                    (_correctorType == EvcCorrectorType.P || _correctorType == EvcCorrectorType.PTZ))
                    return _pressureTest.ActualFactor;

                return 1.0m;
            }
        }

        public IVolumeItems PreTestItems { get; set; }
        public int PulseACount { get; set; }
        public int PulseBCount { get; set; }
        public decimal PulseBScaling { get; }
        public int PulseCCount { get; set; }


        public decimal PulserAScaling { get; }
        public string PulserAUnits { get; }
        public string PulserBUnits { get; }

        public decimal SuperFactor
        {
            get
            {
                if (_superTest != null && _correctorType == EvcCorrectorType.PTZ)
                    return _superTest.ActualFactor;

                return 1.0m;
            }
        }

        /// <summary>
        ///     Temperature factor
        ///     Returns 1 if the Corrector type isn't T or PTZ
        /// </summary>
        public decimal TemperatureFactor
        {
            get
            {
                if (_temperatureTest != null &&
                    (_correctorType == EvcCorrectorType.T || _correctorType == EvcCorrectorType.PTZ))
                    return _temperatureTest.ActualFactor;

                return 1.0m;
            }
        }

        public virtual decimal UncorrectedCalculated => DriveType.UnCorrectedInputVolume(AppliedInput);

        /// <summary>
        ///     EVC Uncorrected calculated volume
        ///     End Uncorrected - Start Uncorrected x Uncorrected Multiplier
        /// </summary>
        public virtual decimal UncorrectedEvcTotal
            => (PostTestItems.UncorrectedReading - PreTestItems.UncorrectedReading) * PreTestItems.UncorrectedMultiplier
        ;

        public virtual bool UncorrectedPassed
            => UncorrectedPercentError < 1 && UncorrectedPercentError > -1;

        /// <summary>
        ///     Uncorrected volume variance percent error
        /// </summary>
        public virtual decimal? UncorrectedPercentError
            =>
                UncorrectedCalculated != 0
                    ? (UncorrectedEvcTotal - UncorrectedCalculated) / UncorrectedCalculated * 100
                    : default(decimal?);

        public static VolumeTestPoint Create(IInstrument instrument, TemperatureTestPoint temperature,
            PressureTestPoint pressure, SuperFactorTestPoint superFactor)
        {
            var volumeTest = new VolumeTestPoint(instrument.CorrectorType, temperature, pressure, superFactor);

            if (instrument.VolumeItems.DriveType == DriveTypeDescripter.Rotary)
                volumeTest.DriveType = new RotaryDrive(instrument.VolumeItems);
            else
                volumeTest.DriveType = new MechanicalDrive(volumeTest);

            return volumeTest;
        }
    }
}