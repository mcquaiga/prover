using Prover.Core.Domain.Enums;
using Prover.Core.Domain.Models.QaTestRuns.QaTestPoints;

namespace Prover.Core.Domain.Models.QaTestRuns
{
    public class QaTestPointDto
    {
        public QaTestPointDto(TestLevel level, PressureTestPointDto pressureTest, TemperatureTestPointDto temperatureTest,
            VolumeTestPointDto volumeTest, SuperFactorTestPointDto superFactorTest)
        {
            Level = level;
            Pressure = pressureTest;
            Temperature = temperatureTest;
            Volume = volumeTest;
            SuperFactor = superFactorTest;
        }

        protected QaTestPointDto()
        {
        }

        public TestLevel Level { get; protected set; }
        public PressureTestPointDto Pressure { get; set; }
        public TemperatureTestPointDto Temperature { get; set; }
        public VolumeTestPointDto Volume { get; set; }
        public SuperFactorTestPointDto SuperFactor { get; private set; }
    }
}