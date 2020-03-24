using System;
using System.ComponentModel.DataAnnotations.Schema;

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
