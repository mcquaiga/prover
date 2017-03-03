using System.Diagnostics.CodeAnalysis;

namespace Prover.Core.Domain.Models.QaTestRuns.QaTestPoints
{
    public class PressureTestPointDto
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public enum PressureUnits
        {
            PSIA,
            PSIG,
            kPa,
            mPa,
            BAR,
            mBAR,
            KGcm2,
            inWC,
            inHG,
            mmHG
        }

        public PressureTestPointDto(PressureUnits units, decimal evcPressure, decimal evcFactor, decimal evcBasePressure,
            decimal gauge, decimal atmosphericGauge)
        {
            Units = units;
            EvcPressure = evcPressure;
            EvcFactor = evcFactor;
            EvcBasePressure = evcBasePressure;
            Gauge = gauge;
            AtmosphericGauge = atmosphericGauge;
        }

        protected PressureTestPointDto()
        {
        }

        public PressureUnits Units { get; set; }

        public decimal EvcPressure { get; set; }
        public decimal EvcFactor { get; set; }
        public decimal EvcBasePressure { get; set; }

        public decimal Gauge { get; set; }
        public decimal AtmosphericGauge { get; set; }
    }
}