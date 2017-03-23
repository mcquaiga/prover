using System;
using Prover.CommProtocol.Common.Instruments;

namespace Prover.Domain.Models.TestRuns
{
    public class PressureTestPoint: IPressureItems
    {
        public PressureTestPoint(decimal gasGauge, int range, string transducerType, decimal evcBase)
        {
            Range = range;
            TransducerType = transducerType;
            Base = evcBase;
            GaugePressure = gasGauge;
            AtmosphericGauge = 0;
        }

        public decimal GaugePressure { get; }
        public decimal AtmosphericGauge { get; }

        public int Range { get; }
        public string TransducerType { get; }
        public decimal Base { get; }

        public decimal AtmPressure { get; set; }
        public decimal Factor { get; set; }
        public decimal GasPressure { get; set; }

        public decimal? PercentError
            => ActualFactor != 0 ? Math.Round((Factor - ActualFactor) / ActualFactor * 100, 2) : default(decimal?);

        public decimal ActualFactor
            => Base != 0 ? GasPressure / Base : 0.0m;

        public void Update(IPressureItems pressureItems)
        {
            AtmPressure = pressureItems.AtmPressure;
            Factor = pressureItems.Factor;
            GasPressure = pressureItems.GasPressure;
        }
    }
}