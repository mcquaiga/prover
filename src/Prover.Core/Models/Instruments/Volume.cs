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
    public class Volume : InstrumentTable
    {
        const int COR_VOLUME = 0;
        const int COR_VOLUME_HIGH_RES = 113;
        const int UNCOR_VOL = 2;
        const int UNCOR_VOL_HIGHRES = 892;
        const int PULSER_A = 93;
        const int PULSER_B = 94;
        const decimal COR_ERROR_THRESHOLD = 1.5m;
        const decimal UNCOR_ERROR_THRESHOLD = 0.1m;
        const decimal METER_DIS_ERROR_THRESHOLD = 1m;

        public Volume(Instrument instrument) : base(instrument.Items.CopyItemsByFilter(i => i.IsVolume == true))
        {
            Instrument = instrument;
            InstrumentId = Instrument.Id;
            AfterTestItems = Instrument.Items.CopyItemsByFilter(x => x.IsVolumeTest == true);
            MeterIndex = MeterIndexInfo.Get((int)Instrument.Items.GetItem(432).GetNumericValue());

            if (Instrument.CorrectorType == CorrectorType.PressureTemperature)
                SuperFactor = new SuperFactor(instrument, TemperatureTest, PressureTest);
        }

        public Volume(Instrument instrument, InstrumentItems afterTestItems) : this(instrument)
        {
            AfterTestItems = afterTestItems;
        }

        public int PulseACount { get; set; }
        [NotMapped]
        public string PulseASelect
        {
            get { return Items.GetItem(PULSER_A).GetDescriptionValue(); }
        }

        public int PulseBCount { get; set; }
        [NotMapped]
        public string PulseBSelect
        {
            get { return Items.GetItem(PULSER_B).GetDescriptionValue(); }
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
        public Instrument Instrument { get; set; }

        public string TestInstrumentData
        {
            get { return JsonConvert.SerializeObject(AfterTestItems.InstrumentValues); }
            set
            {
                Items.InstrumentValues = JsonConvert.DeserializeObject<Dictionary<int, string>>(value);
            }
        }

        [NotMapped]
        public SuperFactor SuperFactor { get; private set; }

        [NotMapped]
        public TemperatureTest TemperatureTest
        {
            get { return Instrument.Temperature.Tests.FirstOrDefault(x => x.IsVolumeTestTemperature); }
        }

        [NotMapped]
        public PressureTest PressureTest
        {
            get { return Instrument.Pressure.Tests.FirstOrDefault(x => x.IsVolumeTestPressure); }
        }

        [NotMapped]
        public InstrumentItems AfterTestItems { get; set; }

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
        public decimal? StartCorrected
        {
            get { return GetHighResolutionValue(Items, COR_VOLUME, COR_VOLUME_HIGH_RES); }
        }

        [NotMapped]
        public decimal? StartUncorrected
        {
            get
            {
                return GetHighResolutionValue(Items, UNCOR_VOL, UNCOR_VOL_HIGHRES);
            }
        }
        

        [NotMapped]
        public decimal? EndCorrected
        {
            get
            {
                if (AfterTestItems == null)
                    return StartCorrected;

                return GetHighResolutionValue(AfterTestItems, COR_VOLUME, COR_VOLUME_HIGH_RES);
            }
        }

        [NotMapped]
        public decimal? EndUncorrected
        {
            get
            {
                return GetHighResolutionValue(AfterTestItems, UNCOR_VOL, UNCOR_VOL_HIGHRES);
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
            get { return Items.GetItem(432).GetDescriptionValue(); }
        }

        [NotMapped]
        public int MeterTypeId
        {
            get { return (int)Items.GetItem(432).GetNumericValue(); }
        }

        [NotMapped]
        public string DriveRateDescription
        {
            get { return Items.GetItem(98).GetDescriptionValue(); }
        }

        [NotMapped]
        public decimal? CorrectedMultiplier
        {
            get { return Items.GetItem(90).GetNumericValue(); }
        }

        [NotMapped]
        public string CorrectedMultiplierDescription
        {
            get { return Items.GetItem(90).GetDescriptionValue(); }
        }

        [NotMapped]
        public decimal? UnCorrectedMultiplier
        {
            get { return Items.GetItem(92).GetNumericValue(); }
        }

        [NotMapped]
        public string UnCorrectedMultiplierDescription
        {
            get { return Items.GetItem(92).GetDescriptionValue(); }
        }

        [NotMapped]
        public decimal? EvcMeterDisplacement
        {
            get { return Items.GetItem(439).GetNumericValue(); }
        }

        [NotMapped]
        public decimal? UnCorrectedInputVolume
        {
            get { return (MeterDisplacement * (decimal)AppliedInput); }
        }

        [NotMapped]
        public decimal? TrueCorrected
        {
            get
            {
                if (Instrument.CorrectorType == CorrectorType.TemperatureOnly && TemperatureTest != null)
                {
                    return (TemperatureTest.ActualFactor * UnCorrectedInputVolume);
                }

                if (Instrument.CorrectorType == CorrectorType.PressureOnly && PressureTest != null)
                {
                    return (PressureTest.ActualFactor * UnCorrectedInputVolume);
                }
                else if (Instrument.CorrectorType == CorrectorType.PressureTemperature)
                {
                    return (PressureTest.ActualFactor * TemperatureTest.ActualFactor * SuperFactor.SuperFactorSquared * UnCorrectedInputVolume);
                }

                return null;
            }
        }

        [NotMapped]
        public decimal MeterDisplacement
        {
            get
            {
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
                if (UnCorrectedInputVolume != 0 && UnCorrectedInputVolume != null)
                {
                    return Math.Round((decimal) (((EvcUncorrected - UnCorrectedInputVolume) / UnCorrectedInputVolume) * 100), 2);
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

        [NotMapped]
        public MeterIndexInfo MeterIndex { get; private set; }

        public static decimal GetHighResolutionValue(InstrumentItems items, int lowResItemNumber, int highResItemNumber)
        {
            return JoinLowResHighResReading(items.GetItem(lowResItemNumber).GetNumericValue(), items.GetItem(highResItemNumber).GetNumericValue());
        }

        public static decimal GetHighResFractionalValue(decimal highResValue)
        {
            if (highResValue == 0) return 0;

            var highResString = Convert.ToString(highResValue);
            var pointLocation = highResString.IndexOf(".", StringComparison.Ordinal);

            if (highResValue > 0 && pointLocation > -1)
            {
                var result = highResString.Substring(pointLocation, highResString.Length - pointLocation);

                return Convert.ToDecimal(result);
            }

            return 0;
        }

        public static decimal GetHighResolutionItemValue(int lowResValue, decimal highResValue)
        {
            var fractional = GetHighResFractionalValue(highResValue);
            return lowResValue + fractional;
        }

        public static decimal JoinLowResHighResReading(decimal? lowResValue, decimal? highResValue)
        {
            if (!lowResValue.HasValue || !highResValue.HasValue)
                throw new ArgumentNullException(nameof(lowResValue) + " & " + nameof(highResValue));

            return GetHighResolutionItemValue((int)lowResValue.Value, highResValue.Value);
        }
    }    
}
