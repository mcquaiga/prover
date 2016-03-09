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
    public class Temperature : InstrumentTable
    {
        const int TEMP_UNITS = 89;
        const int BASE_TEMP = 34;

        public Temperature(Instrument instrument) :
            base(instrument.Items.CopyItemsByFilter(x => x.IsTemperature == true))
        {
            Instrument = instrument;
            InstrumentId = instrument.Id;
        }

        public virtual ICollection<TemperatureTest> Tests { get; set; } = new Collection<TemperatureTest>();

        public Guid InstrumentId { get; set; }

        [Required]
        public virtual Instrument Instrument { get; set; }

        #region Not Mapped Properties
        [NotMapped]
        public string Range
        {
            get { return "-40 - 150 " + Units; }
        }

        [NotMapped]
        public string Units => Instrument.Items.GetItem(TEMP_UNITS).GetDescriptionValue();

        [NotMapped]
        public decimal? EvcBase => Instrument.Items.GetItem(BASE_TEMP).GetNumericValue();

        [NotMapped]
        public bool HasPassed
        {
            get { return Tests.All(x=> x.HasPassed); }
        }
        #endregion

        public TemperatureTest AddTemperatureTest()
        {
            if (Tests.Count() >= 3)
                throw new NotSupportedException("Only 3 test instances are supported.");

            var test = new TemperatureTest(this, (TemperatureTest.Level)Tests.Count());
            Tests.Add(test);

            return test;
        }
    }
}
