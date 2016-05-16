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
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
        }
    }

    public abstract class ProverTable : BaseEntity
    {
        protected ProverTable() : base()
        {
        
        }

        public string InstrumentData
        {
            get { return JsonConvert.SerializeObject(ItemValues); }
            set
            {
                ItemValues = JsonConvert.DeserializeObject<Dictionary<int, string>>(value);
            }
        }

        [NotMapped]
        public Dictionary<int, string> ItemValues { get; set; }
    }
}
