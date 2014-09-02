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
    public sealed class Volume : ItemsBase
    {
        private string _data;

        public Volume(Instrument instrument)
        {
            Instrument = instrument;
            Id = new Guid();
            Items = Item.LoadItems(Instrument.Type).Where(x => x.IsVolume == true).ToList();
        }

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

        public Instrument Instrument { get; set; }
        public TemperatureTest TemperatureTest { get; set; }

        public double AppliedInput { get; set; }

        [NotMapped]
        public double? StartCorrected
        {
            get { return NumericValue(0); }
        }

        [NotMapped]
        public double? StartUncorrected
        {
            get { return NumericValue(2); }
        }

        [NotMapped]
        public double? EndCorrected
        {
            get { return NumericValue(0); }
        }

        [NotMapped]
        public double? EndUncorrected
        {
            get { return NumericValue(2); }
        }

        [NotMapped]
        public EvcType CorrectionType
        {
            get
            {
                //Pressure Live
                if (DescriptionValue(109).ToLower() == "live" && DescriptionValue(111).ToLower() == "live")
                {
                    return EvcType.PressureTemperature;
                }
                
                if (DescriptionValue(109).ToLower() == "live")
                {
                    return EvcType.Pressure;
                }

                return EvcType.Temperature;
            }
        }

        [NotMapped]
        public string MeterType
        {
            get { return DescriptionValue(432); }
        }

        [NotMapped]
        public string DriveRateDescription
        {
            get { return DescriptionValue(98); }
        }

        [NotMapped]
        public string CorrectedMultiplierDescription
        {
            get { return DescriptionValue(90); }
        }

        [NotMapped]
        public string UnCorrectedMultiplierDescription
        {
            get { return DescriptionValue(92); }
        }

        [NotMapped]
        public double? EvcMeterDisplacement
        {
            get { return NumericValue(439); }
        }


        //TODO
        [NotMapped]
        public double? MeterDisplacement
        {
            get { return NumericValue(439); }
        }

        public double? EvcCorrected
        {
            get { return 0; }
        }

        public double? EvcUncorrected
        {
            get { return 0; }
        }

        public double? UnCorrectedPercentError
        {
            get { return 0.00; }
        }

        public double? CorrectedPercentError
        {
            get { return 0.00; }
        }
    }
}
