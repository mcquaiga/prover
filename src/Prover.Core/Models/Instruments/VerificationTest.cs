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
            BuildCorrectorTypes(Instrument, hasVolumeTest);
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
                if (Instrument.CorrectorType == CorrectorType.TemperatureOnly && TemperatureTest != null)
                    return TemperatureTest.HasPassed && VolumeTest.HasPassed;

                if (Instrument.CorrectorType == CorrectorType.PressureOnly && PressureTest != null)
                    return PressureTest.HasPassed && VolumeTest.HasPassed;

                if (Instrument.CorrectorType == CorrectorType.PressureTemperature && PressureTest != null && TemperatureTest != null)
                    return TemperatureTest.HasPassed && VolumeTest.HasPassed && PressureTest.HasPassed;

                return false;
            }
        }

        private void BuildCorrectorTypes(Instrument instrument, bool hasVolumeTest = false)
        {
            if (instrument.CorrectorType == CorrectorType.PressureOnly)
            {
                PressureTest = new PressureTest(this);
            }

            if (instrument.CorrectorType == CorrectorType.TemperatureOnly)
            {
                TemperatureTest = new TemperatureTest(this, GetGaugeTemp(TestNumber));
            }

            if (instrument.CorrectorType == CorrectorType.PressureTemperature)
            {
                PressureTest = new PressureTest(this);
                TemperatureTest = new TemperatureTest(this, GetGaugeTemp(TestNumber));
                SuperFactorTest = new SuperFactorTest(this);
            }

            if (hasVolumeTest)
                VolumeTest = new VolumeTest(this);
        }

        private static decimal GetGaugeTemp(int testNumber)
        {
            switch (testNumber)
            {
                case 0:
                    return 90m;
                case 1:
                    return 60m;
                default:
                    return 32m;
            }
        }
    }
}
