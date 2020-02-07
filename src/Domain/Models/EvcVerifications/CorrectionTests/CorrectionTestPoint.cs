using System.Collections.Generic;
using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Models.EvcVerifications.DriveTypes;

namespace Domain.Models.EvcVerifications.CorrectionTests
{
    public sealed class CorrectionTestPoint : AggregateRoot, IAssertPassFail
    {
        public IDeviceWithValues Device { get; }

        public FrequencyTest FrequencyTest { get; private set; }

        public bool Passed
        {
            get
            {
                var volumePass = VolumeTest == null || VolumeTest.Passed;

                switch (Device.Composition)
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

        public CorrectionTestPoint(IDeviceWithValues evcDevice, int testLevel)
        {
            TestNumber = testLevel;
            Device = evcDevice;
        }

        internal void AddFrequency()
        {
            FrequencyTest = new FrequencyTest(this);
        }

        internal void AddPressure(IPressureItems pressureItems, decimal pressureGaugePercent)
        {
            PressureTest = new PressureTest(pressureItems, pressureGaugePercent);
        }

        internal void AddSuperFactor(ISuperFactorItems superItems)
        {
            SuperFactorTest = new SuperFactorTest(superItems, PressureTest, TemperatureTest);
        }

        internal void AddTemperature(ITemperatureItems tempItems, decimal temperatureGauge)
        {
            TemperatureTest = new TemperatureTest(tempItems, temperatureGauge);
        }

        internal void AddVolume(List<CorrectionTestDefinition.MechanicalUncorrectedTestLimit> mechanicalDriveTestLimits)
        {
            if (Device.Volume.DriveType is MechanicalDrive)
            {
                VolumeTest = new MechanicalVolumeTest(Device.Volume, mechanicalDriveTestLimits);
            }
            else
            {
                VolumeTest = new VolumeTest(Device.Volume);
            }
        }

        private CorrectionTestPoint()
        {
        }

        //public override void OnInitializing()
        //{
        //    base.OnInitializing();

        //    if (Instrument.CompositionType == EvcCorrectorType.PTZ)
        //        SuperFactorTest = new SuperFactorTest(this);
        //}
    }
}