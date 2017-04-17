using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Prover.Core.Models.Instruments
{
    public class VerificationTest : BaseEntity
    {
        public VerificationTest()
        {
        }

        public VerificationTest(int testNumber, Instrument instrument, bool hasVolumeTest = false)
        {
            TestNumber = testNumber;
            Instrument = instrument;
        }

        public VerificationTest(int testNumber, PressureTest pressureTest, TemperatureTest temperatureTest, VolumeTest volumeTest)
        {
            TestNumber = testNumber;

            PressureTest = pressureTest;
            if (PressureTest != null)
            {
                PressureTest.VerificationTest = this;
                PressureTest.OnInitializing();
            }

            TemperatureTest = temperatureTest;
            if (TemperatureTest != null)
            {
                TemperatureTest.VerificationTest = this;
            }

            VolumeTest = volumeTest;
            if (VolumeTest != null)
            {
                VolumeTest.VerificationTest = this;
            }

        }

        public VerificationTest(int testNumber, Instrument instrument, PressureTest pressureTest,
            TemperatureTest temperatureTest, SuperFactorTest superFactorTest, VolumeTest volumeTest)
            : this(testNumber, instrument)
        {
            PressureTest = pressureTest;
            TemperatureTest = temperatureTest;
            SuperFactorTest = superFactorTest;
            VolumeTest = volumeTest;
        }

        public int TestNumber { get; set; }

        public Guid InstrumentId { get; set; }

        [Required]
        public Instrument Instrument { get; set; }

        public PressureTest PressureTest { get; set; }

        public TemperatureTest TemperatureTest { get; set; }

        public VolumeTest VolumeTest { get; set; }

        [NotMapped]
        public SuperFactorTest SuperFactorTest { get; set; }

        [NotMapped, JsonIgnore]
        public bool HasPassed
        {
            get
            {
                if (Instrument?.CompositionType == CorrectorType.T && TemperatureTest != null)
                    return TemperatureTest.HasPassed && (VolumeTest == null || VolumeTest.HasPassed);

                if (Instrument?.CompositionType == CorrectorType.P && PressureTest != null)
                    return PressureTest.HasPassed && (VolumeTest == null || VolumeTest.HasPassed);

                if (Instrument?.CompositionType == CorrectorType.PTZ && PressureTest != null && TemperatureTest != null)
                    return TemperatureTest.HasPassed && PressureTest.HasPassed &&
                           (VolumeTest == null || VolumeTest.HasPassed);

                return false;
            }
        }

        public override void OnInitializing()
        {
            base.OnInitializing();
            Initialize();
        }

        private void Initialize()
        {
            if (TemperatureTest != null && PressureTest != null)
                SuperFactorTest = new SuperFactorTest(this);

            TemperatureTest?.OnInitializing();
            PressureTest?.OnInitializing();
            VolumeTest?.OnInitializing();
        }
    }
}