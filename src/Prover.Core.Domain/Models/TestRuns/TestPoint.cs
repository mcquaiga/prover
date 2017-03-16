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

        public static TestPoint Create(TestLevel level, IInstrument instrument, List<ItemValue> itemValues)
        {
            var correctorType = instrument.CorrectorType(itemValues);

            var testPoint = new TestPoint()
            {
                Level = level
            };

            if (correctorType == EvcCorrectorType.T || correctorType == EvcCorrectorType.PTZ)
            {
                testPoint.Temperature = new TemperatureTestPoint(
                        instrument.GetItemValue(ItemCodes.Temperature.Base, itemValues).NumericValue,
                        (TemperatureUnits)Enum.Parse(typeof(TemperatureUnits), instrument.GetItemValue(ItemCodes.Temperature.Units, itemValues).Description),
                        TempGauges[level]);
            }

            if (correctorType == EvcCorrectorType.P || correctorType == EvcCorrectorType.PTZ)
            {
                testPoint.Pressure = new PressureTestPoint(
                    PressureGauges[level],
                    (int)instrument.GetItemValue(ItemCodes.Pressure.Range, itemValues).NumericValue,
                    instrument.GetItemValue(ItemCodes.Pressure.TransducerType, itemValues).Description,
                    instrument.GetItemValue(ItemCodes.Pressure.Base, itemValues).NumericValue);
            }

            if (correctorType == EvcCorrectorType.PTZ)
            {
                testPoint.SuperFactor = new SuperFactorTestPoint()
                {
                    TemperatureTest = testPoint.Temperature,
                    PressureTest = testPoint.Pressure,
                    SpecGr = instrument.GetItemValue(ItemCodes.Super.SpecGr, itemValues).NumericValue,
                    Co2 = instrument.GetItemValue(ItemCodes.Super.Co2, itemValues).NumericValue,
                    N2 = instrument.GetItemValue(ItemCodes.Super.N2, itemValues).NumericValue
                };
            }

            if (level == TestLevel.Level1)
            {
                testPoint.Volume = new VolumeTestPoint(correctorType);
            }

            return testPoint;
        }
    }
}