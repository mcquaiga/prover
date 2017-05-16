#region

using System;
using System.Collections.Generic;

#endregion

namespace Prover.Core.Shared.DTO.TestRuns
{
    public class TemperatureTestDto : TestDtoBase
    {
        public TemperatureTestDto()
        {
        }

        public TemperatureTestDto(Guid id) : base(id, null)
        {
        }

        public TemperatureTestDto(Guid id, Dictionary<string, string> itemData, decimal gaugeTemperature,
            decimal actualFactor) : base(id, itemData)
        {
            GaugeTemperature = gaugeTemperature;
            //ActualFactor = actualFactor;
        }

        //public decimal ActualFactor { get; set; }
        public decimal GaugeTemperature { get; set; }
    }
}