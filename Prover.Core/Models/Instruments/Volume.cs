using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Prover.Core.Models.Instruments
{
    public class Volume : ItemsBase
    {
        public enum EvcType
        {
            PressureTemperature,
            Pressure,
            Temperature
        }

        private string _data;
        
        private Dictionary<int, string> _testInstrumentValues;

        public Volume(Instrument instrument)
        {
            Instrument = instrument;
            Items = Item.LoadItems(Instrument.Type).Where(x => x.IsVolume == true).ToList();
            AfterTestItems = Item.LoadItems(Instrument.Type).Where(x => x.IsVolumeTest == true).ToList();
        }
        public int PulseACount { get; set; }
        public int PulseBCount { get; set; }
        public double AppliedInput { get; set; }

        public Guid InstrumentId { get; set; }
        [ForeignKey("InstrumentId")]
        public Instrument Instrument { get; set; }

        public Guid TemperatureTestId { get; set; }
        [ForeignKey("TemperatureTestId"),]
        public TemperatureTest TemperatureTest { get; set; }
        
        public string TestInstrumentData
        {
            get { return JsonConvert.SerializeObject(TestInstrumentValues); }
            set
            {
                _data = value;
                _testInstrumentValues = JsonConvert.DeserializeObject<Dictionary<int, string>>(value);
            }
        }

        [NotMapped]
        public ICollection<Item> AfterTestItems { get; set; }

        [NotMapped]
        public double? EvcCorrected
        {
            get { return Math.Round((double)((EndCorrected - StartCorrected) * CorrectedMultiplier), 4); }
        }

        [NotMapped]
        public double? EvcUncorrected
        {
            get { return Math.Round((double)((EndUncorrected - StartUncorrected) * UnCorrectedMultiplier), 4); }

        }

        [NotMapped]
        public Dictionary<int, string> TestInstrumentValues
        {
            get { return _testInstrumentValues; }
            set { _testInstrumentValues = value; }
        }

        [NotMapped]
        public double? StartCorrected
        {
            get { return NumericValue(0) + ParseHighResReading(NumericValue(113)); }
        }

        [NotMapped]
        public double? StartUncorrected
        {
            get { return NumericValue(2) + ParseHighResReading(NumericValue(892)); }
        }

        [NotMapped]
        public double? EndCorrected
        {
            get { return NumericValue(0, AfterTestItems) + ParseHighResReading(NumericValue(113, AfterTestItems)); }
        }

        [NotMapped]
        public double? EndUncorrected
        {
            get { return NumericValue(2, AfterTestItems) + ParseHighResReading(NumericValue(892, AfterTestItems)); }
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
        public double? CorrectedMultiplier
        {
            get { return NumericValue(90); }
        }

        [NotMapped]
        public string CorrectedMultiplierDescription
        {
            get { return DescriptionValue(90); }
        }

        [NotMapped]
        public double? UnCorrectedMultiplier
        {
            get { return NumericValue(92); }
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

        [NotMapped]
        public double? TrueUncorrected
        {
            get { return (MeterDisplacement * AppliedInput); }
        }

        [NotMapped]
        public double? TrueCorrected
        {
            get
            {
                if (TemperatureTest == null) return null;
                if (CorrectionType == EvcType.Temperature)
                {
                    return (TemperatureTest.ActualFactor * TrueUncorrected);
                }
                return null;
            }
        }

        [NotMapped]
        public double? MeterDisplacement
        {
            get { return NumericValue(439); }
        }

        [NotMapped]
        public double? UnCorrectedPercentError
        {
            get
            {
                if (TrueUncorrected != 0)
                {
                    return Math.Round((double) (((EvcUncorrected - TrueUncorrected) / TrueUncorrected) * 100), 2);
                }
                return null;
            }
        }

        [NotMapped]
        public double? CorrectedPercentError
        {
            get
            {
                if (TrueCorrected != 0 && TrueCorrected != null)
                {
                    return Math.Round((double)(((EvcCorrected - TrueCorrected) / TrueCorrected) * 100), 2);    
                }

                return null;
            }
        }
    }
}
