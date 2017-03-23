using System;
using System.Collections.Generic;
using System.Linq;
using Prover.CommProtocol.Common.Instruments;
using Prover.CommProtocol.Common.Items;
using Prover.Shared.Enums;

namespace Prover.Domain.Models.TestRuns
{
    public class TestPoint
    {
        private static readonly Dictionary<TestLevel, int> TempGauges = new Dictionary<TestLevel, int>()
        {
            { TestLevel.Level1, 32 },
            { TestLevel.Level2, 60 },
            { TestLevel.Level3, 90 }
        };

        private static readonly Dictionary<TestLevel, decimal> PressureGauges = new Dictionary<TestLevel, decimal>()
        {
            { TestLevel.Level1, 0.80m },
            { TestLevel.Level2, 0.50m },
            { TestLevel.Level3, 0.20m }
        };

        public TestLevel Level { get; set; }
        public PressureTestPoint Pressure { get; set; }
        public TemperatureTestPoint Temperature { get; set; }
        public SuperFactorTestPoint SuperFactor { get; protected set; }
        public VolumeTestPoint Volume { get; set; }

        public static TestPoint Create(TestLevel level, IInstrument instrument)
        {
            var correctorType = instrument.CorrectorType();

            var testPoint = new TestPoint()
            {
                Level = level
            };

            if (correctorType == EvcCorrectorType.T || correctorType == EvcCorrectorType.PTZ)
            {
                testPoint.Temperature = new TemperatureTestPoint(instrument.TemperatureItems.Base, instrument.TemperatureItems.Units, TempGauges[level]);
            }

            if (correctorType == EvcCorrectorType.P || correctorType == EvcCorrectorType.PTZ)
            {
                testPoint.Pressure = new PressureTestPoint(
                    PressureGauges[level] * instrument.PressureItems.Range, 
                    instrument.PressureItems.Range, 
                    instrument.PressureItems.TransducerType, 
                    instrument.PressureItems.Base);
            }

            if (correctorType == EvcCorrectorType.PTZ)
            {
                testPoint.SuperFactor = new SuperFactorTestPoint()
                {
                    TemperatureTest = testPoint.Temperature,
                    PressureTest = testPoint.Pressure,
                    SpecGr = instrument.SuperFactorItems.SpecGr,
                    Co2 = instrument.SuperFactorItems.Co2,
                    N2 = instrument.SuperFactorItems.N2
                };
            }

            if (level == TestLevel.Level1)
            {
                testPoint.Volume = new VolumeTestPoint(
                    correctorType, 
                    testPoint.Temperature, 
                    testPoint.Pressure,
                    testPoint.SuperFactor);
            }

            return testPoint;
        }
    }
}