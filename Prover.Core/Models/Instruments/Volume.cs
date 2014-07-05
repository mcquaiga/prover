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
    public class Volume
    {
        private string _data;

        public enum EvcType
        {
            PressureTemperature,
            Pressure,
            Temperature
        }
        
        [Key]
        public Guid Id { get; set; }

        public int PulseACount { get; set; }
        public int PulseBCount { get; set; }

        public string Data
        {
            get { return JsonConvert.SerializeObject(Items); }
            set { _data = value; }
        }

        public IEnumerable<Item> Items { get; set; }
        
        [NotMapped]
        public TemperatureTest TemperatureTest { get; set; }

        [NotMapped]
        public EvcType CorrectionType { get; set; }

        [NotMapped]
        public decimal EvcMeterDisplacement { get; set; }

    }
}
