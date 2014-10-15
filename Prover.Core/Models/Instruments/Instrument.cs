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
            TestDateTime = DateTime.Now;
            Type = type;
            Items = Item.LoadItems(type);
        }

        public Instrument() : this(InstrumentType.MiniMax)
        {
        }

        public double? SerialNumber
        {
            get { return NumericValue(62); }
        }
        public DateTime TestDateTime { get; set; }
        public InstrumentType Type { get; set; }

        [NotMapped]
        public string TypeString
        {
            get { return Type.ToString(); }
        }

        public Guid CertificateGuid { get; set; }

        public virtual Temperature Temperature { get; set; }
        public virtual Volume Volume { get; set; }

        [NotMapped]
        public decimal? FirmwareVersion
        {
            get
            {
                if (InstrumentValues != null)
                    return Convert.ToDecimal(InstrumentValues.FirstOrDefault(x => x.Key == 122).Value);
                return null;
            }
        }

        [NotMapped]
        public double? PulseAScaling
        {
            get { return NumericValue(56); }
        }

        [NotMapped]
        public string PulseASelect
        {
            get { return DescriptionValue(93); }
        }

        [NotMapped]
        public double? PulseBScaling
        {
            get { return NumericValue(57); }
        }

        [NotMapped]
        public string PulseBSelect
        {
            get { return DescriptionValue(94); }
        }

        [NotMapped]
        public double? SiteNumber1
        {
            get { return NumericValue(200); }
        }

        [NotMapped]
        public double? SiteNumber2
        {
            get { return NumericValue(201); }
        }

        [NotMapped]
        public bool HasPassed
        {
            get { return this.Temperature.HasPassed && this.Volume.HasPassed; }
        }
    }    
}
