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

    public class Pressure : ItemsBase
    {
        private const string BASE_PRESS = "BASE_PRESS";
        private const string ATM_PRESS = "ATM_PRESS";
        private const string PRESS_UNITS = "PRESS_UNITS";
        private const string PRESS_RANGE = "PRESS_RANGE";
        private const string TRANSDUCER_TYPE = "TRANSDUCER_TYPE";

        public Pressure(Instrument instrument)
        {
            Items = Item.LoadItems(instrument.Type).Where(x => x.IsPressure == true).ToList();
            Instrument = instrument;
            InstrumentId = instrument.Id;
            Tests = new Collection<PressureTest>()
            {
                new PressureTest(this, PressureTest.PressureLevel.Low),
                new PressureTest(this, PressureTest.PressureLevel.Medium),
                new PressureTest(this, PressureTest.PressureLevel.High)
            };
        }

        public Guid InstrumentId { get; set; }

        [Required]
        public virtual Instrument Instrument { get; set; }

        public virtual ICollection<PressureTest> Tests { get; set; }

        [NotMapped]
        public string Units
        {
            get { return DescriptionValue(GetItemNumber(PRESS_UNITS)); }
        }

        [NotMapped]
        public decimal? EvcBase
        {
            get
            {
                return NumericValue(GetItemNumber(BASE_PRESS));
            }
        }

        [NotMapped]
        public decimal? EvcAtmospheric
        {
            get
            {
                return NumericValue(GetItemNumber(ATM_PRESS));
            }
        }

        [NotMapped]
        public decimal? EvcPressureRange
        {
            get
            {
                return NumericValue(GetItemNumber(PRESS_RANGE));
            }
        }

        [NotMapped]
        public TransducerType TransducerType
        {
            get
            {
                return (TransducerType)(int)NumericValue(GetItemNumber(TRANSDUCER_TYPE));
            }
        }

        [NotMapped]
        public bool HasPassed
        {
            get { return Tests.All(x => x.HasPassed); }
        }
    }
}
