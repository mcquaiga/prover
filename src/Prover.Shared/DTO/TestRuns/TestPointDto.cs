using System;
using Prover.Shared.Domain;
using Prover.Shared.Enums;

namespace Prover.Shared.DTO.TestRuns
{
    public class TestPointDto : Entity<Guid>
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