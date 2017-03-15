using Prover.Shared.Enums;

namespace Prover.Domain.Models.TestRuns
{
    public class TestPoint
    {
        public TestLevel Level { get; set; }
        public PressureTestPoint Pressure { get; set; }
        public TemperatureTestPoint Temperature { get; set; }
        public SuperFactorTestPoint SuperFactor { get; protected set; }
        public VolumeTestPoint Volume { get; set; }
    }
}