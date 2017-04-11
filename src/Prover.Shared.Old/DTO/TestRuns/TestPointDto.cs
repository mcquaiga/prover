using System;
using System.Collections.Generic;
using Prover.Shared.Domain;
using Prover.Shared.Enums;

namespace Prover.Shared.DTO.TestRuns
{
    public class TestPointDto : Entity<Guid>
    {
        public TestPointDto(Guid id) : base(id)
        {
        }

        public TestLevel Level { get; set; }
        public PressureTestDto Pressure { get; set; }
        public SuperFactorTestDto SuperFactor { get; set; }
        public TemperatureTestDto Temperature { get; set; }
        public VolumeTestDto Volume { get; set; }

    }

    public class PressureTestDto
    {
        public PressureTestDto(Dictionary<string, string> itemsData, decimal gauge, decimal atmosphericGauge)
        {
            ItemsData = itemsData;
            Gauge = gauge;
            AtmosphericGauge = atmosphericGauge;
        }

        public decimal AtmosphericGauge { get; set; }
        public decimal Gauge { get; set; }
        public Dictionary<string, string> ItemsData { get; set; }
    }

    public class SuperFactorTestDto
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

        public decimal ActualFactor { get; set; }
        public decimal EvcUnsqrFactor { get; set; }
        public decimal GaugePressure { get; set; }

        public decimal GaugeTemp { get; set; }
        public decimal SuperFactorSquared { get; protected set; }
    }

    public class TemperatureTestDto
    {
        public TemperatureTestDto(TemperatureUnits units, decimal evcTemperature, decimal evcFactor,
            decimal evcBaseTemperature,
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

        public decimal ActualFactor { get; set; }
        public decimal EvcBaseTemperature { get; set; }
        public decimal EvcFactor { get; set; }
        public decimal EvcTemperature { get; set; }

        public decimal GaugeTemperature { get; set; }
        public TemperatureUnits Units { get; set; }
    }

    public class VolumeTestDto
    {
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

        public decimal AppliedInput { get; set; }
        public string DriveTypeDiscriminator { get; set; }
        public decimal EvcEndCorrected { get; set; }
        public decimal EvcEndUncorrected { get; set; }
        public decimal EvcStartCorrected { get; set; }
        public decimal EvcStartUncorrected { get; set; }
        public int PulseACount { get; set; }
        public int PulseBCount { get; set; }

        //Corrected
        public decimal TrueCorrected { get; set; }

        //UnCorrected
        public decimal TrueUncorrected { get; set; }
    }
}