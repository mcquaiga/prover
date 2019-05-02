using Core.Domain;
using Devices.Core;
using Devices.Core.Interfaces;
using System.Collections.Generic;

namespace Module.EvcVerification.Models.CorrectionTests
{
    public sealed class CorrectionTestPoint : AggregateRoot, IAssertPassFail
    {
        private CorrectionTestPoint()
        {
        }

        public CorrectionTestPoint(IDevice evcDevice, int testLevel)
        {
            TestNumber = testLevel;
            Device = evcDevice;
        }

        public IDevice Device { get; }

        public FrequencyTest FrequencyTest { get; private set; }

        public bool Passed
        {
            get
            {
                var volumePass = VolumeTest == null || VolumeTest.Passed;

                switch (Device.CompositionType)
                {
                    case CompositionType.TemperatureOnly:
                        return TemperatureTest?.Passed != false && volumePass;

                    case CompositionType.PressureOnly:
                        return PressureTest?.Passed != false && volumePass;

                    case CompositionType.PressureTemperature:
                        return TemperatureTest?.Passed != false
                            && PressureTest?.Passed != false
                            && SuperFactorTest?.Passed != false
                            && volumePass;
                }

                return false;
            }
        }

        public PressureTest PressureTest { get; private set; }

        public SuperFactorTest SuperFactorTest { get; private set; }

        public TemperatureTest TemperatureTest { get; private set; }

        public int TestNumber { get; set; }

        public VolumeTest VolumeTest { get; private set; }

        internal void AddFrequency()
        {
            FrequencyTest = new FrequencyTest(this);
        }

        internal void AddPressure(decimal pressureGaugePercent)
        {
            PressureTest = new PressureTest(Device.PressureItems, pressureGaugePercent);
        }

        internal void AddSuperFactor()
        {
            SuperFactorTest = new SuperFactorTest(Device.SuperFactorItems, PressureTest, TemperatureTest);
        }

        internal void AddTemperature(decimal temperatureGauge)
        {
            TemperatureTest = new TemperatureTest(this, temperatureGauge);
        }

        internal void AddVolume(List<CorrectionTestDefinition.MechanicalUncorrectedTestLimit> mechanicalDriveTestLimits)
        {
            VolumeTest = new VolumeTest(this);
        }

        //public override void OnInitializing()
        //{
        //    base.OnInitializing();

        //    if (Instrument.CompositionType == EvcCorrectorType.PTZ)
        //        SuperFactorTest = new SuperFactorTest(this);
        //}
    }
}