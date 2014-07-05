using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Core.Models.Instruments;

namespace Prover.Core.Models.Certificates
{
    public class Certificate
    {
        public Guid Id { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string TestedBy { get; set; }
        public long Number { get; set; }

        public virtual IEnumerable<Instrument> Instruments { get; set; }
 
    }
}
