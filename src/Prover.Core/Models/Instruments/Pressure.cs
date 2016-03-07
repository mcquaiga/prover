using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Models.Instruments
{
    public enum PressureUnits
    {
        PSIG = 0,
        PSIA = 1,
        kPa = 2,
        mPa = 3,
        BAR = 4,
        mBAR = 5,
        KGcm2 = 6,
        inWC = 7,
        inHG = 8,
        mmHG = 9
    }

    public enum TransducerType
    {
        Gauge = 0,
        Absolute = 1
    }

    public class Pressure : InstrumentTable
    {
        private const string BASE_PRESS = "BASE_PRESS";
        private const string ATM_PRESS = "ATM_PRESS";
        private const string PRESS_UNITS = "PRESS_UNITS";
        private const string PRESS_RANGE = "PRESS_RANGE";
        private const string TRANSDUCER_TYPE = "TRANSDUCER_TYPE";

        public Pressure(Instrument instrument) : base(instrument.Items.FindItems(i => i.IsPressure == true))
        {
            Instrument = instrument;
            InstrumentId = instrument.Id;
        }

        public Guid InstrumentId { get; set; }

        [Required]
        public virtual Instrument Instrument { get; set; }

        public virtual ICollection<PressureTest> Tests { get; set; }

        #region Not Mapped Properties
        [NotMapped]
        public string Units
        {
            get { return Items.GetItem(PRESS_UNITS).GetDescriptionValue(); }
        }

        [NotMapped]
        public decimal? EvcBase
        {
            get
            {
                return Items.GetItem(BASE_PRESS).GetNumericValue();
            }
        }

        [NotMapped]
        public decimal? EvcAtmospheric
        {
            get
            {
                return Items.GetItem(ATM_PRESS).GetNumericValue();
            }
        }

        [NotMapped]
        public decimal? EvcPressureRange
        {
            get
            {
                return Items.GetItem(PRESS_RANGE).GetNumericValue();
            }
        }

        [NotMapped]
        public TransducerType TransducerType
        {
            get
            {
                return (TransducerType)Items.GetItem(TRANSDUCER_TYPE).GetNumericValue();
            }
        }

        [NotMapped]
        public bool HasPassed
        {
            get { return Tests.All(x => x.HasPassed); }
        }
        #endregion

        public PressureTest AddTest()
        {
            if (Tests.Count() >= 3)
                throw new NotSupportedException("Only 3 test instances are supported.");

            var test = new PressureTest(this, (PressureTest.PressureLevel)Tests.Count());
            Tests.Add(test);

            return test;
        }
    }
}
