using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using Prover.Core.Extensions;

namespace Prover.Core.Models.Instruments
{
    public abstract class InstrumentTable
    {
        protected InstrumentTable()
        {
            Id = Guid.NewGuid();
        }

        protected InstrumentTable(InstrumentItems items) : this()
        {
            Items = items;
        }

        [Key]
        public Guid Id { get; set; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string InstrumentData
        {
            get { return JsonConvert.SerializeObject(Items.InstrumentValues); }
            set
            {
                Items.InstrumentValues = JsonConvert.DeserializeObject<Dictionary<int, string>>(value);
            }
        }

        [NotMapped]
        public InstrumentItems Items { get; set; }
    }
}
