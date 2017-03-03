using System.Diagnostics.CodeAnalysis;
using Prover.Core.Domain.Enums;

namespace Prover.Core.Domain.Models.QaTestRuns.QaTestPoints
{
    public class TemperatureTestPointDto
    {
        public TemperatureTestPointDto(TemperatureUnits units, decimal evcTemperature, decimal evcFactor, decimal evcBaseTemperature,
            decimal gaugeTemperature, decimal actualFactor)
        {
            Units = units;
            EvcTemperature = evcTemperature;
            EvcFactor = evcFactor;
            EvcBaseTemperature = evcBaseTemperature;
            GaugeTemperature = gaugeTemperature;
            ActualFactor = actualFactor;
        }

        protected TemperatureTestPointDto()
        {
        }

        public TemperatureUnits Units { get; set; }
        public decimal EvcTemperature { get; set; }
        public decimal EvcFactor { get; set; }
        public decimal EvcBaseTemperature { get; set; }

        public decimal GaugeTemperature { get; set; }
        public decimal ActualFactor { get; set; }
    }
}