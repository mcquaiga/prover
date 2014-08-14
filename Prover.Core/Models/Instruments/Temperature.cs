using System;
using System.Collections.Generic;
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
            Id = Guid.NewGuid();

        }
      
        public virtual ICollection<TemperatureTest> Tests { get; set; }
        public virtual Instrument Instrument { get; set; }

        [NotMapped]
        public string Range
        {
            get { return "-40 - 150 " + Units; }
        }

        [NotMapped]
        public string Units { get; set; }
    }
}
