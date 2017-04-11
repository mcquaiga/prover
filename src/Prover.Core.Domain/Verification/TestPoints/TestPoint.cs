using System;
using System.Collections.Generic;
using Prover.Domain.Instrument;
using Prover.Domain.Instrument.Items;
using Prover.Domain.Verification.TestPoints.Pressure;
using Prover.Domain.Verification.TestPoints.SuperFactor;
using Prover.Domain.Verification.TestPoints.Temperature;
using Prover.Domain.Verification.TestPoints.Volume;
using Prover.Shared.Domain;
using Prover.Shared.Enums;

namespace Prover.Domain.Verification.TestPoints
{
    public class TestPoint : Entity<Guid>
    {
        private static readonly Dictionary<TestLevel, double> PressureGauges = new Dictionary<TestLevel, double>
        {
            {TestLevel.Level1, 0.80},
            {TestLevel.Level2, 0.50},
            {TestLevel.Level3, 0.20}
        };

        private static readonly Dictionary<TestLevel, int> TempGauges = new Dictionary<TestLevel, int>
        {
            {TestLevel.Level1, 32},
            {TestLevel.Level2, 60},
            {TestLevel.Level3, 90}
        };

        public IInstrument Instrument { get; }

        private TestPoint(Guid id, IInstrument instrument, TestLevel level)
            : base(id)
        {
            Instrument = instrument;
            Level = level;
        }

        public TestPoint() : base(Guid.NewGuid())
        {
        }

        public TestPoint(Guid id, IInstrument instrument, TestLevel level, PressureTestPoint pressure, TemperatureTestPoint temperature, VolumeTestPoint volume)
            : this(id, instrument, level)
        {
            Pressure = pressure;
            Temperature = temperature;
            Volume = volume;
        }

        public TestPoint(IInstrument instrument, TestLevel level)
            : this(Guid.NewGuid(), instrument, level)
        {
            Pressure = CreatePressureTest(level, instrument);
            Temperature = CreateTemperatureTest(level, instrument);
            Volume = CreateVolumeTest(level, instrument);
        }

        public TestRun.TestRun TestRun { get; set; }
        public TestLevel Level { get; protected set; }
        public PressureTestPoint Pressure { get; protected set; }
        public SuperFactorTestPoint SuperFactor
        {
            get
            {
                if (Instrument.CorrectorType != EvcCorrectorType.PTZ) return null;

                return new SuperFactorTestPoint(Instrument.SuperFactorItems, Temperature.GaugeTemperature,
                    Pressure.GasPressure, Pressure.EvcItems.UnsqrFactor);
            }
        }
        public TemperatureTestPoint Temperature { get; protected set; }
        public VolumeTestPoint Volume { get; set; }

        public void Update(IPressureItems pressureTestItems, ITemperatureItems temperatureTestItems, IVolumeItems volumePreTestItems, IVolumeItems volumePostTestItems)
        {
            if (Pressure != null)
                Pressure.EvcItems = pressureTestItems;

            if (Temperature != null)
                Temperature.EvcItems = temperatureTestItems;

            if (Volume != null)
                Volume.Update(volumePreTestItems, volumePostTestItems, Volume.AppliedInput, Temperature.ActualFactor, Pressure.ActualFactor, SuperFactor.ActualFactor);
        }

        private static PressureTestPoint CreatePressureTest(TestLevel level, IInstrument instrument)
        {
            PressureTestPoint pressure = null;
            if (instrument.CorrectorType == EvcCorrectorType.P || instrument.CorrectorType == EvcCorrectorType.PTZ)
            {
                pressure = new PressureTestPoint()
                {
                    EvcItems = instrument.PressureItems
                };

                pressure.SetGaugeValues(PressureGauges[level] * instrument.PressureItems.Range, 0);
            }

            return pressure;
        }

        private static TemperatureTestPoint CreateTemperatureTest(TestLevel level, IInstrument instrument)
        {
            TemperatureTestPoint temperature = null;
            if (instrument.CorrectorType == EvcCorrectorType.T || instrument.CorrectorType == EvcCorrectorType.PTZ)
                temperature = new TemperatureTestPoint(
                    TempGauges[level],
                    instrument.TemperatureItems);
            return temperature;
        }

        private static VolumeTestPoint CreateVolumeTest(TestLevel level, IInstrument instrument)
        {
            VolumeTestPoint volume = null;
            if (level == TestLevel.Level1)
                volume = new VolumeTestPoint(instrument.VolumeItems);

            return volume;
        }
    }
}