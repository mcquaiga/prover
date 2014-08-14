using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Newtonsoft.Json;
using Prover.Core.Communication;
using Prover.SerialProtocol;

namespace Prover.Core.Models.Instruments
{
    public enum InstrumentType
    {
        MiniMax = 4,
        MiniAt = 3,
        Ec300 = 7
    }

    public class Instrument : ItemsBase
    {
        public Instrument(InstrumentType type)
        {
            Id = Guid.NewGuid();
            TestDateTime = DateTime.Now;
            Type = type;
            Items = Item.LoadItems(type);
        }

        public Instrument() : this(InstrumentType.MiniMax)
        {
        }

        public long? SerialNumber
        {
            get {
                if (InstrumentValues != null)
                    return Convert.ToInt64(InstrumentValues.FirstOrDefault(x => x.Key == 62).Value);
                return null;
            }
        }

        public DateTime TestDateTime { get; set; }
        public InstrumentType Type { get; set; }
        public Guid CertificateGuid { get; set; }

        [NotMapped]
        public virtual Temperature Temperature { get; set; }
    }    
}
