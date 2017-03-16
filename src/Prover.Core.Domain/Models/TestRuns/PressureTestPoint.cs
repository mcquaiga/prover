using System;

namespace Prover.Domain.Models.TestRuns
{
    public class PressureTestPoint
    {
        public PressureTestPoint(decimal gaugePercentage, int range, string transducerType, decimal evcBase)
        {
            Range = range;
            TransducerType = transducerType;
            EvcBase = evcBase;
            GasGauge = gaugePercentage * Range;
        }

        public decimal GasGauge { get; set; }
        public int Range { get; set; }
        public string TransducerType { get; set; }
        public decimal EvcBase { get; set; }

        public decimal EvcFactor { get; set; }
        public decimal AtmosphericGauge { get; set; }

        public decimal GasPressure
        {
            get
            {
                decimal result;
                if (TransducerType == "Gauge")
                    result = GasGauge;
                else
                    result = GasGauge + AtmosphericGauge;

                return decimal.Round(result, 2);
            }
        }

        public decimal? PercentError
            => ActualFactor != 0 ? Math.Round((EvcFactor - ActualFactor) / ActualFactor * 100, 2) : default(decimal?);

        public decimal ActualFactor
            => EvcBase != 0 ? GasPressure / EvcBase : 0.0m;
    }
}