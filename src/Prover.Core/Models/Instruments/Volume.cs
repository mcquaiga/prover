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
using NLog;
using Prover.Core.Extensions;

namespace Prover.Core.Models.Instruments
{
    public class Volume : ItemsBase
    {
        const decimal COR_ERROR_THRESHOLD = 1.5m;
        const decimal UNCOR_ERROR_THRESHOLD = 0.1m;
        const decimal METER_DIS_ERROR_THRESHOLD = 1m;

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
                    MeterDisplacement = Convert.ToDecimal(x.Attribute("MeterDisplacement").Value)
                }).ToList();
            //
            
            MeterIndex = indexes.FirstOrDefault();
            if (MeterIndex != null) LogManager.GetCurrentClassLogger().Info(string.Format("Meter Id:{0}; Type: {1}; Displacement: {2}", MeterIndex.Id, MeterIndex.Description, MeterIndex.MeterDisplacement));
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
        public decimal? EvcCorrected
        {
            get { return Math.Round((decimal)((EndCorrected - StartCorrected) * CorrectedMultiplier), 4); }
        }

        [NotMapped]
        public decimal? EvcUncorrected
        {
            get { return Math.Round((decimal)((EndUncorrected - StartUncorrected) * UnCorrectedMultiplier), 4); }
        }

        [NotMapped]
        public Dictionary<int, string> TestInstrumentValues
        {
            get { return _testInstrumentValues; }
            set { _testInstrumentValues = value; }
        }

        [NotMapped]
        public decimal? StartCorrected
        {
            get { return ParseHighResReading((int)NumericValue(0), NumericValue(113).Value); }
        }

        [NotMapped]
        public decimal? StartUncorrected
        {
            get
            {
                return ParseHighResReading((int)NumericValue(2), NumericValue(892).Value);
            }
        }

        [NotMapped]
        public decimal? EndCorrected
        {
            get
            {
                if (TestInstrumentValues == null)
                    return StartCorrected;

                var lowResValue = (int)NumericValue(0, AfterTestItems, TestInstrumentValues);
                var highResValue = NumericValue(113, AfterTestItems, TestInstrumentValues).Value;

                return ParseHighResReading(lowResValue, highResValue);
            }
        }

        [NotMapped]
        public decimal? EndUncorrected
        {
            get
            {
                if (TestInstrumentValues == null)
                    return StartUncorrected;

                var lowResValue = (int)NumericValue(2, AfterTestItems, TestInstrumentValues);
                var highResValue = NumericValue(892, AfterTestItems, TestInstrumentValues).Value;

                return ParseHighResReading(lowResValue, highResValue);
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
        public decimal? MeterTypeId
        {
            get { return NumericValue(432); }
        }

        [NotMapped]
        public string DriveRateDescription
        {
            get { return DescriptionValue(98); }
        }

        [NotMapped]
        public decimal? CorrectedMultiplier
        {
            get { return NumericValue(90); }
        }

        [NotMapped]
        public string CorrectedMultiplierDescription
        {
            get { return DescriptionValue(90); }
        }

        [NotMapped]
        public decimal? UnCorrectedMultiplier
        {
            get { return NumericValue(92); }
        }

        [NotMapped]
        public string UnCorrectedMultiplierDescription
        {
            get { return DescriptionValue(92); }
        }

        [NotMapped]
        public decimal? EvcMeterDisplacement
        {
            get { return NumericValue(439); }
        }

        [NotMapped]
        public decimal? TrueUncorrected
        {
            get { return (MeterDisplacement * (decimal)AppliedInput); }
        }

        [NotMapped]
        public decimal? TrueCorrected
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
        public decimal MeterDisplacement
        {
            get
            {
                if (MeterIndex == null)
                    LoadMeterIndex();

                if (MeterIndex != null)
                    return MeterIndex.MeterDisplacement.Value;

                return 0;
            }
        }

        [NotMapped]
        public decimal UnCorrectedPercentError
        {
            get
            {
                if (TrueUncorrected != 0 && TrueUncorrected != null)
                {
                    return Math.Round((decimal) (((EvcUncorrected - TrueUncorrected) / TrueUncorrected) * 100), 2);
                }
                else
                {
                    return 0;
                }
            }
        }

        [NotMapped]
        public decimal CorrectedPercentError
        {
            get
            {
                if (TrueCorrected != 0 && TrueCorrected != null)
                {
                    return Math.Round((decimal)(((EvcCorrected - TrueCorrected) / TrueCorrected) * 100), 2);    
                }
                else
                {
                    return 0;
                }
            }
        }

        [NotMapped]
        public decimal MeterDisplacementPercentError
        {
            get
            {
                if (MeterDisplacement != 0)
                {
                    return Math.Round((decimal)(((EvcMeterDisplacement - MeterDisplacement) / MeterDisplacement) * 100), 2);
                }
                return 0;
            }
        }

        [NotMapped]
        public bool CorrectedHasPassed
        {
            get { return CorrectedPercentError.IsBetween(COR_ERROR_THRESHOLD); }
        }

        [NotMapped]
        public bool UnCorrectedHasPassed
        {
            get { return (UnCorrectedPercentError.IsBetween(UNCOR_ERROR_THRESHOLD)); }
        }

        [NotMapped]
        public bool MeterDisplacementHasPassed
        {
            get { return (MeterDisplacementPercentError.IsBetween(METER_DIS_ERROR_THRESHOLD)); }
        }

        [NotMapped]
        public bool HasPassed
        {
            get 
            {
                return CorrectedHasPassed && UnCorrectedHasPassed && MeterDisplacementHasPassed;
            }
        }
    }


    public class MeterIndexInfo
    {
        public MeterIndexInfo() { }
        public MeterIndexInfo(int id, string description, int unCorPulsesX10, int unCorPulsesX100, decimal? meterDisplacement)
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
        public decimal? MeterDisplacement { get; set; }
    }
    
}
