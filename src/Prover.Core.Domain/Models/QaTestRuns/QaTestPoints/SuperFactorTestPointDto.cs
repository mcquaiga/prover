namespace Prover.Core.Domain.Models.QaTestRuns.QaTestPoints
{
    public class SuperFactorTestPointDto
    {
        public SuperFactorTestPointDto(decimal gaugeTemp, decimal gaugePressure, decimal evcUnsqrFactor, decimal actualFactor,
            decimal superFactorSquared)
        {
            GaugeTemp = gaugeTemp;
            GaugePressure = gaugePressure;
            EvcUnsqrFactor = evcUnsqrFactor;
            ActualFactor = actualFactor;
            SuperFactorSquared = superFactorSquared;
        }

        public decimal GaugeTemp { get; set; }
        public decimal GaugePressure { get; set; }
        public decimal EvcUnsqrFactor { get; set; }
        public decimal ActualFactor { get; set; }
        public decimal SuperFactorSquared { get; protected set; }
    }
}