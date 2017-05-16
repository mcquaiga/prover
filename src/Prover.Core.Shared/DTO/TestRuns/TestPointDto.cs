#region

using System;
using Prover.Core.Shared.Domain;
using Prover.Core.Shared.Enums;

#endregion

namespace Prover.Core.Shared.DTO.TestRuns
{
    public class TestPointDto : Entity
    {
        public TestPointDto() : base(Guid.NewGuid())
        {
        }

        public TestPointDto(Guid id) : base(id)
        {
        }

        public TestRunDto TestRun { get; set; }
        public TestLevel Level { get; set; }
        public PressureTestDto Pressure { get; set; }
        public SuperFactorTestDto SuperFactor { get; set; }
        public TemperatureTestDto Temperature { get; set; }
        public VolumeTestDto Volume { get; set; }
    }
}