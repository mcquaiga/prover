using System.Collections.Generic;
using Prover.Core.DomainPortable.Instrument;
using Prover.Domain.Verification.TestPoints.Pressure;
using Prover.Domain.Verification.TestPoints.SuperFactor;
using Prover.Domain.Verification.TestPoints.Temperature;
using Prover.Domain.Verification.TestPoints.Volume;

namespace Prover.Domain.Verification.TestPoints
{
    public class TestPoint
    {
        private static readonly Dictionary<TestLevel, decimal> PressureGauges = new Dictionary<TestLevel, decimal>
        {
            {TestLevel.Level1, 0.80m},
            {TestLevel.Level2, 0.50m},
            {TestLevel.Level3, 0.20m}
        };

        private static readonly Dictionary<TestLevel, int> TempGauges = new Dictionary<TestLevel, int>
        {
            {TestLevel.Level1, 32},
            {TestLevel.Level2, 60},
            {TestLevel.Level3, 90}
        };

        public TestPoint(TestLevel level, PressureTestPoint pressure, SuperFactorTestPoint superFactor,
            TemperatureTestPoint temperature, VolumeTestPoint volume)
        {
            Level = level;
            Pressure = pressure;
            SuperFactor = superFactor;
            Temperature = temperature;
            Volume = volume;
        }

        public TestLevel Level { get; protected set; }
        public PressureTestPoint Pressure { get; protected set; }
        public SuperFactorTestPoint SuperFactor { get; protected set; }
        public TemperatureTestPoint Temperature { get; protected set; }
        public VolumeTestPoint Volume { get; set; }

        public static TestPoint Create(TestLevel level, IInstrument instrument)
        {
            var temperature = CreateTemperatureTest(level, instrument);
            var pressure = CreatePressureTest(level, instrument);
            var superFactor = CreateSuperFactor(instrument, temperature, pressure);
            var volume = CreateVolumeTest(level, instrument, temperature, pressure, superFactor);

            return new TestPoint(level, pressure, superFactor, temperature, volume);
        }

        private static PressureTestPoint CreatePressureTest(TestLevel level, IInstrument instrument)
        {
            PressureTestPoint pressure = null;
            if (instrument.CorrectorType == EvcCorrectorType.P || instrument.CorrectorType == EvcCorrectorType.PTZ)
                pressure = new PressureTestPoint(
                    PressureGauges[level] * instrument.PressureItems.Range,
                    instrument.PressureItems);
            return pressure;
        }

        private static SuperFactorTestPoint CreateSuperFactor(IInstrument instrument, TemperatureTestPoint temperature,
            PressureTestPoint pressure)
        {
            SuperFactorTestPoint superFactor = null;
            if (instrument.CorrectorType == EvcCorrectorType.PTZ)
                superFactor = new SuperFactorTestPoint(
                    instrument.SuperFactorItems,
                    pressure,
                    temperature);

            return superFactor;
        }

        private static TemperatureTestPoint CreateTemperatureTest(TestLevel level, IInstrument instrument)
        {
            TemperatureTestPoint temperature = null;
            if (instrument.CorrectorType == EvcCorrectorType.T || instrument.CorrectorType == EvcCorrectorType.PTZ)
                temperature = new TemperatureTestPoint(
                    TempGauges[level],
                    instrument.TemperatureItems);
            return temperature;
        }

        private static VolumeTestPoint CreateVolumeTest(TestLevel level, IInstrument instrument,
            TemperatureTestPoint temperature, PressureTestPoint pressure, SuperFactorTestPoint superFactor)
        {
            VolumeTestPoint volume = null;
            if (level == TestLevel.Level1)
                volume = VolumeTestPoint.Create(instrument, temperature, pressure, superFactor);

            return volume;
        }
    }
}