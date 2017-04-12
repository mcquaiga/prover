using System;
using System.Collections.Generic;

namespace Prover.Shared.DTO.TestRuns
{
    public class SuperFactorTestDto : TestDtoBase
    {
        public SuperFactorTestDto() { }

        public SuperFactorTestDto(Guid id, Dictionary<string, string> itemData, decimal gaugeTemperature, decimal gaugePressure,
            decimal actualFactor)
            : base(id, itemData)
        {
            GaugeTemperature = gaugeTemperature;
            GaugePressure = gaugePressure;
            ActualFactor = actualFactor;
        }

        public decimal ActualFactor { get; set; }
        public decimal GaugePressure { get; set; }
        public decimal GaugeTemperature { get; set; }
        public double UnsquaredFactor { get; set; }
    }
}