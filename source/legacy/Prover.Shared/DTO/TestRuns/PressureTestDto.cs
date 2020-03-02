using System;
using System.Collections.Generic;

namespace Prover.Shared.DTO.TestRuns
{
    public class PressureTestDto : TestDtoBase
    {
        public PressureTestDto(Guid id, Dictionary<string, string> itemData, decimal gaugePressure,
            decimal atmosphericGauge)
            : base(id, itemData)
        {
            GaugePressure = gaugePressure;
            AtmosphericGauge = atmosphericGauge;
        }

        public PressureTestDto(Guid id) : base(id) { }

        public PressureTestDto() { }

        public decimal AtmosphericGauge { get; set; }
        public decimal GaugePressure { get; set; }
    }
}