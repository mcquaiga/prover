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
    public class Temperature : ItemsBase
    {
        public Temperature()
        {
            Items = Item.LoadItems(InstrumentType.MiniMax).Where(x => x.IsTemperature == true).ToList();
        }

        public Temperature(Instrument instrument)
        {
            Instrument = instrument;
            InstrumentId = instrument.Id;
            Items = Item.LoadItems(Instrument.Type).Where(x => x.IsTemperature == true).ToList();
            Tests = new Collection<TemperatureTest>()
            {
                new TemperatureTest(this, Instrument.Type, TemperatureTest.Level.Low),
                new TemperatureTest(this, Instrument.Type, TemperatureTest.Level.Medium),
                new TemperatureTest(this, Instrument.Type, TemperatureTest.Level.High)
            };
        }

        public virtual ICollection<TemperatureTest> Tests { get; set; }

        public Guid InstrumentId { get; set; }
        [Required]
        public virtual Instrument Instrument { get; set; }

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
        public decimal? EvcBase
        {
            get
            {
                return NumericValue(34);
            }      
        }

        [NotMapped]
        public bool HasPassed
        {
            get { return Tests.All(x=> x.HasPassed); }
        }
    }
}
