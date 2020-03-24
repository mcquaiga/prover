using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Models.Instruments
{
    public class GroupedInstruments
    {
        public Guid ParentId { get; set; }
        public virtual Instrument Parent { get; set; }
                
        public Guid ChildId { get; set; }

        [ForeignKey("ChildId")]
        public virtual Instrument Child { get; set; }
    }
}
