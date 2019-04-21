using Core.Domain;
using System.Collections.Generic;

namespace Module.EvcVerification.Models.CorrectionTestAggregate
{
    public sealed class CorrectionTest : AggregateRoot
    {
        #region Constructors

        public CorrectionTest(IEvcDevice evcDevice, int testLevel)
        {
            TestNumber = testLevel;
            EvcDevice = evcDevice;
        }

        #endregion

        #region Properties

        public IEvcDevice EvcDevice { get; }

        public FrequencyTest FrequencyTest { get; private set; }

        public bool HasPassed
        {
            get
            {
                //var volumePass = VolumeTest == null || VolumeTest.HasPassed;

                //if (Instrument.CompositionType == EvcCorrectorType.T && TemperatureTest != null)
                //    return TemperatureTest.HasPassed && volumePass;

                //if (Instrument.CompositionType == EvcCorrectorType.P && PressureTest != null)
                //    return PressureTest.HasPassed && volumePass;

                //if (Instrument.CompositionType == EvcCorrectorType.PTZ && PressureTest != null && TemperatureTest != null)
                //    return TemperatureTest.HasPassed && PressureTest.HasPassed && SuperFactorTest.HasPassed && volumePass;

                return false;
            }
        }

        public PressureTest PressureTest { get; private set; }

        public SuperFactorTest SuperFactorTest { get; set; }

        public TemperatureTest TemperatureTest { get; private set; }

        public int TestNumber { get; set; }

        public VolumeTest VolumeTest { get; private set; }

        #endregion

        #region Methods

        internal void AddPressure(double pressureGaugePercent)
        {
            PressureTest = new PressureTest(EvcDevice.PressureItems, pressureGaugePercent);
        }

        #endregion

        internal void AddFrequency()
        {
            FrequencyTest = new FrequencyTest(this);
        }

        internal void AddSuperFactor()
        {
            SuperFactorTest = new SuperFactorTest(EvcDevice.SuperFactorItems, EvcDevice.PressureItems, EvcDevice.TemperatureItems);
        }

        internal void AddTemperature(double temperatureGauge)
        {
            TemperatureTest = new TemperatureTest(this, temperatureGauge);
        }

        internal void AddVolume(List<CorrectionTestDefinition.MechanicalUncorrectedTestLimit> mechanicalDriveTestLimits)
        {
            VolumeTest = new VolumeTest(this, mechanicalDriveTestLimits);
        }

        private CorrectionTest() { }

        //public override void OnInitializing()
        //{
        //    base.OnInitializing();

        //    if (Instrument.CompositionType == EvcCorrectorType.PTZ)
        //        SuperFactorTest = new SuperFactorTest(this);
        //}
    }
}