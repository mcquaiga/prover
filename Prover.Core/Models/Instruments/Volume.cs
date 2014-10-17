using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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

        public Volume()
        {
            Items = Item.LoadItems(InstrumentType.MiniMax).Where(x => x.IsVolume == true).ToList();
            AfterTestItems = Item.LoadItems(InstrumentType.MiniMax).Where(x => x.IsVolumeTest == true).ToList();
            LoadMeterIndex();
        }

        public Volume(Instrument instrument)
        {
            Instrument = instrument;
            InstrumentId = Instrument.Id;
            Items = Item.LoadItems(Instrument.Type).Where(x => x.IsVolume == true).ToList();
            AfterTestItems = Item.LoadItems(Instrument.Type).Where(x => x.IsVolumeTest == true).ToList();
        }
        [NotMapped]
        public MeterIndexInfo MeterIndex { get; set; } 

        public void LoadMeterIndex()
        {
            var xDoc = XDocument.Load("MeterIndexes.xml");
            var indexes = 
                (from x in xDoc.Descendants("value")
                 where Convert.ToInt32(x.Attribute("id").Value) == MeterTypeId
                select new MeterIndexInfo()
                {
                    Id = Convert.ToInt32(x.Attribute("id").Value),
                    Description = x.Attribute("description").Value,
                    UnCorPulsesX10 = Convert.ToInt32(x.Attribute("UnCorPulsesX10").Value),
                    UnCorPulsesX100 = Convert.ToInt32(x.Attribute("UnCorPulsesX100").Value),
                    MeterDisplacement = Convert.ToDouble(x.Attribute("MeterDisplacement").Value)
                }).ToList();
            //

            MeterIndex = indexes.FirstOrDefault();
        }

        public int PulseACount { get; set; }
        [NotMapped]
        public string PulseASelect
        {
            get { return DescriptionValue(93); }
        }

        public int PulseBCount { get; set; }
        [NotMapped]
        public string PulseBSelect
        {
            get { return DescriptionValue(94); }
        }

        [NotMapped]
        public int UncPulseCount
        {
            get
            {
                if (PulseASelect == "UncVol")
                    return PulseACount;
                return PulseBCount;
            }
        }

        [NotMapped]
        public int CorPulseCount
        {
            get
            {
                if (PulseASelect == "CorVol")
                    return PulseACount;
                return PulseBCount;
            }
        }

        public int MaxUnCorrected()
        {
            if (UnCorrectedMultiplier == 10)
                return MeterIndex.UnCorPulsesX10;

            if (UnCorrectedMultiplier == 100)
                return MeterIndex.UnCorPulsesX100;

            return 10; //Low standard number if we can't find anything
        }

        public double AppliedInput { get; set; }

        public Guid InstrumentId { get; set; }
        [Required]
        public virtual Instrument Instrument { get; set; }
        
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
        public TemperatureTest TemperatureTest
        {
            get { return Instrument.Temperature.Tests.FirstOrDefault(x => x.IsVolumeTestTemperature); }
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
            get
            {
                if (TestInstrumentValues != null)
                    return NumericValue(0, AfterTestItems, TestInstrumentValues) + ParseHighResReading(NumericValue(113, AfterTestItems, TestInstrumentValues));
                return NumericValue(0) + ParseHighResReading(NumericValue(113));
            }
        }

        [NotMapped]
        public double? EndUncorrected
        {
            get
            {   
                if (TestInstrumentValues != null)
                    return NumericValue(2, AfterTestItems, TestInstrumentValues) + ParseHighResReading(NumericValue(892, AfterTestItems, TestInstrumentValues));
                return NumericValue(2) + ParseHighResReading(NumericValue(892));
            }
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
        public string MeterTypeDescription
        {
            get { return MeterIndex.Description; }
        }

        [NotMapped]
        public string MeterType
        {
            get { return DescriptionValue(432); }
        }

        [NotMapped]
        public double? MeterTypeId
        {
            get { return NumericValue(432); }
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
            get
            {
                if (MeterIndex != null)
                    return MeterIndex.MeterDisplacement;
                return null;
            }
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

        [NotMapped]
        public bool HasPassed
        {
            get { return (CorrectedPercentError < 1 && CorrectedPercentError > -1) && (UnCorrectedPercentError < 1 && UnCorrectedPercentError > -1); }
        }
    }


    public class MeterIndexInfo
    {
        public MeterIndexInfo() { }
        public MeterIndexInfo(int id, string description, int unCorPulsesX10, int unCorPulsesX100, double? meterDisplacement)
        {
            Id = id;
            Description = description;
            UnCorPulsesX10 = unCorPulsesX10;
            UnCorPulsesX100 = unCorPulsesX100;
            MeterDisplacement = meterDisplacement;
        }

        public int Id { get; set; }
        public string Description { get; set; }
        public int UnCorPulsesX10 { get; set; }
        public int UnCorPulsesX100 { get; set; }
        public double? MeterDisplacement { get; set; }
    }
    
}
