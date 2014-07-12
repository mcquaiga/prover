using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Models.Instruments
{
    public class Temperature
    {
        public Temperature()
        {
            Id = Guid.NewGuid();

        }

        public Guid Id { get; set; }
        public string InstrumentData { get; set; }

        public virtual ICollection<TemperatureTest> Tests { get; set; }

        [NotMapped]
        public IEnumerable<InstrumentValue> InstrumentValues { get; set; }

        [NotMapped]
        public Instrument Instrument { get; set; }


        [NotMapped]
        public List<Item> Items
        {
            get { return Instrument.Items.Where(x => x.IsTemperature == true).ToList(); }
        }

        [NotMapped]
        public string Range
        {
            get { return "-40 - 150 " + Units; }
        }

        [NotMapped]
        public string Units { get; set; }
    }
}
