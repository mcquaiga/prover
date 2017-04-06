using System.Collections.Generic;
using Prover.Domain.Models.Instruments;
using Prover.Domain.Models.VerificationTests.PTZ;
using Prover.Shared.Common;
using Prover.Shared.Enums;

namespace Prover.Domain.Models.VerificationTests
{
    public class TestPoint : Entity
    {
        private static readonly Dictionary<TestLevel, int> TempGauges = new Dictionary<TestLevel, int>
        {
            {TestLevel.Level1, 32},
            {TestLevel.Level2, 60},
            {TestLevel.Level3, 90}
        };

        private static readonly Dictionary<TestLevel, decimal> PressureGauges = new Dictionary<TestLevel, decimal>
        {
            {TestLevel.Level1, 0.80m},
            {TestLevel.Level2, 0.50m},
            {TestLevel.Level3, 0.20m}
        };

        public TestLevel Level { get; protected set; }
        public PressureTestPoint Pressure { get; protected set; }
        public TemperatureTestPoint Temperature { get; protected set; }
        public SuperFactorTestPoint SuperFactor { get; protected set; }
        public VolumeTestPoint Volume { get; set; }

        public static TestPoint Create(TestLevel level, IInstrument instrument)
        {
            var testPoint = new TestPoint
            {
                Level = level
            };

            if (instrument.CorrectorType == EvcCorrectorType.T || instrument.CorrectorType == EvcCorrectorType.PTZ)
                testPoint.Temperature = new TemperatureTestPoint(
                    TempGauges[level],
                    instrument.TemperatureItems);

            if (instrument.CorrectorType == EvcCorrectorType.P || instrument.CorrectorType == EvcCorrectorType.PTZ)
                testPoint.Pressure = new PressureTestPoint(
                    PressureGauges[level] * instrument.PressureItems.Range,
                    instrument.PressureItems);

            if (instrument.CorrectorType == EvcCorrectorType.PTZ)
                testPoint.SuperFactor = new SuperFactorTestPoint(
                    instrument.SuperFactorItems,
                    testPoint.Pressure,
                    testPoint.Temperature);

            if (level == TestLevel.Level1)
                testPoint.Volume = VolumeTestPoint.Create(instrument, testPoint);

            return testPoint;
        }
    }
}