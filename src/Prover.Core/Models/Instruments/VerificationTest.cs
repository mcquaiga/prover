using Prover.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Models.Instruments
{
    public class VerificationTest : BaseEntity
    {
        public VerificationTest() { }

        public VerificationTest(int testNumber, Instrument instrument, bool hasVolumeTest = false)
        {
            TestNumber = testNumber;
            Instrument = instrument;
            InstrumentId = Instrument.Id;
            //BuildCorrectorTypes(Instrument, hasVolumeTest);
        }

        public VerificationTest(int testNumber, Instrument instrument, PressureTest pressureTest, TemperatureTest temperatureTest, SuperFactorTest superTest, VolumeTest volumeTest) : this(testNumber, instrument)
        {
            PressureTest = pressureTest;
            TemperatureTest = temperatureTest;
            SuperFactorTest = superTest;
            VolumeTest = volumeTest;
        }

        public int TestNumber { get; set; }

        public Guid InstrumentId { get; set; }
        [Required]
        public virtual Instrument Instrument { get; set; }
        public virtual PressureTest PressureTest { get; set; }
        public virtual TemperatureTest TemperatureTest { get; set; }
        public virtual VolumeTest VolumeTest { get; set; }

        [NotMapped]
        public virtual SuperFactorTest SuperFactorTest { get; set; }
        [NotMapped]
        public bool HasPassed
        {
            get
            {
                if (Instrument.CompositionType == CorrectorType.T && TemperatureTest != null)
                    return TemperatureTest.HasPassed && (VolumeTest == null || VolumeTest.HasPassed);

                if (Instrument.CompositionType == CorrectorType.P && PressureTest != null)
                    return PressureTest.HasPassed && (VolumeTest == null || VolumeTest.HasPassed);

                if (Instrument.CompositionType == CorrectorType.PTZ && PressureTest != null && TemperatureTest != null)
                    return TemperatureTest.HasPassed && (VolumeTest == null || VolumeTest.HasPassed) && PressureTest.HasPassed;

                return false;
            }
        }
    }
}
