using Prover.Shared.Common;
using Prover.Shared.Enums;

namespace Prover.Shared.DTO.TestRuns
{
    public class TestPointDto : Entity
    {
        public TestLevel Level { get; set; }
        public PressureTestDto Pressure { get; set; }
        public TemperatureTestDto Temperature { get; set; }
        public VolumeTestDto Volume { get; set; }
        public SuperFactorTestDto SuperFactor { get; set; }
    }

    public class PressureTestDto : Entity
    {
        public PressureUnits Units { get; set; }

        public decimal EvcPressure { get; set; }
        public decimal EvcFactor { get; set; }
        public decimal EvcBasePressure { get; set; }
        public decimal Gauge { get; set; }
        public decimal AtmosphericGauge { get; set; }

        public PressureTestDto(PressureUnits units, decimal evcPressure, decimal evcFactor, decimal evcBasePressure,
            decimal gauge, decimal atmosphericGauge)
        {
            Units = units;
            EvcPressure = evcPressure;
            EvcFactor = evcFactor;
            EvcBasePressure = evcBasePressure;
            Gauge = gauge;
            AtmosphericGauge = atmosphericGauge;
        }

        protected PressureTestDto()
        {
        }
    }

    public class SuperFactorTestDto : Entity
    {
        public SuperFactorTestDto(decimal gaugeTemp, decimal gaugePressure, decimal evcUnsqrFactor, decimal actualFactor,
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

    public class TemperatureTestDto : Entity
    {
        public TemperatureUnits Units { get; set; }
        public decimal EvcTemperature { get; set; }
        public decimal EvcFactor { get; set; }
        public decimal EvcBaseTemperature { get; set; }

        public decimal GaugeTemperature { get; set; }
        public decimal ActualFactor { get; set; }

        public TemperatureTestDto(TemperatureUnits units, decimal evcTemperature, decimal evcFactor, decimal evcBaseTemperature,
            decimal gaugeTemperature, decimal actualFactor)
        {
            Units = units;
            EvcTemperature = evcTemperature;
            EvcFactor = evcFactor;
            EvcBaseTemperature = evcBaseTemperature;
            GaugeTemperature = gaugeTemperature;
            ActualFactor = actualFactor;
        }

        protected TemperatureTestDto()
        {
        }
    }

    public class VolumeTestDto : Entity
    {
        public int PulseACount { get; set; }
        public int PulseBCount { get; set; }
        public decimal AppliedInput { get; set; }
        public string DriveTypeDiscriminator { get; set; }

        //UnCorrected
        public decimal TrueUncorrected { get; set; }
        public decimal EvcStartUncorrected { get; set; }
        public decimal EvcEndUncorrected { get; set; }

        //Corrected
        public decimal TrueCorrected { get; set; }
        public decimal EvcStartCorrected { get; set; }
        public decimal EvcEndCorrected { get; set; }

        public VolumeTestDto(int pulseACount, int pulseBCount, decimal appliedInput, string driveTypeDiscriminator,
            decimal trueUncorrected, decimal evcStartUncorrected, decimal evcEndUncorrected, decimal trueCorrected,
            decimal evcStartCorrected, decimal evcEndCorrected)
        {
            PulseACount = pulseACount;
            PulseBCount = pulseBCount;
            AppliedInput = appliedInput;
            DriveTypeDiscriminator = driveTypeDiscriminator;
            TrueUncorrected = trueUncorrected;
            EvcStartUncorrected = evcStartUncorrected;
            EvcEndUncorrected = evcEndUncorrected;
            TrueCorrected = trueCorrected;
            EvcStartCorrected = evcStartCorrected;
            EvcEndCorrected = evcEndCorrected;
        }

        protected VolumeTestDto()
        {
        }
    }
}