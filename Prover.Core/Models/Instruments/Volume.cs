using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Prover.Core.Models.Instruments
{
    public class Volume : ItemsBase
    {
        private string _data;

        public enum EvcType
        {
            PressureTemperature,
            Pressure,
            Temperature
        }

        public int PulseACount { get; set; }
        public int PulseBCount { get; set; }

        public string Data
        {
            get { return JsonConvert.SerializeObject(Items); }
            set { _data = value; }
        }

        public virtual Instrument Instrument { get; set; }
        public virtual TemperatureTest TemperatureTest { get; set; }

        [NotMapped]
        public EvcType CorrectionType { get; set; }

        [NotMapped]
        public decimal EvcMeterDisplacement { get; set; }

    }
}
