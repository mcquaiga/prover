using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Models.Instruments
{
    public class Temperature : ItemsBase
    {
        public Temperature(Instrument instrument)
        {
            Instrument = instrument;
            Items = Item.LoadItems(Instrument.Type).Where(x => x.IsTemperature == true).ToList();
            Tests = new Collection<TemperatureTest>()
            {
                new TemperatureTest(Instrument, TemperatureTest.Level.Low),
                new TemperatureTest(Instrument, TemperatureTest.Level.Medium),
                new TemperatureTest(Instrument, TemperatureTest.Level.High)
            };
        }
        
        public virtual ICollection<TemperatureTest> Tests { get; set; }

        public Guid InstrumentId { get; set; }
        [ForeignKey("InstrumentId")]
        public Instrument Instrument { get; set; }

        [NotMapped]
        public string Range
        {
            get { return "-40 - 150 " + Units; }
        }

        [NotMapped]
        public string Units
        {
            get { return DescriptionValue(89); }
        }

        [NotMapped]
        public double? EvcBase
        {
            get
            {
                return NumericValue(34);
            }      
        }

    }
}
