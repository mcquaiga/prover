using Prover.Core.Models;
using Prover.Core.Models.Instruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Extensions
{
    public static class VolumeHelpers
    {
        const int COR_VOLUME = 0;
        const int COR_VOLUME_HIGH_RES = 113;
        const int UNCOR_VOL = 2;
        const int UNCOR_VOL_HIGHRES = 892;
        const int PULSER_A = 93;
        const int PULSER_B = 94;

        
        public static string PulseASelect(this Instrument instrument)
        {
            return instrument.ItemDetails.GetItem(PULSER_A).GetDescriptionValue(instrument.ItemValues);
        }
        
        public static string PulseBSelect(this Instrument instrument)
        {
            return instrument.ItemDetails.GetItem(PULSER_B).GetDescriptionValue(instrument.ItemValues);
        }

        public static decimal? EvcCorrected(this Instrument instrument, Dictionary<int, string> beforeItems, Dictionary<int, string> afterItems)
        {
            if (afterItems == null || beforeItems == null) return null;

            return Math.Round((decimal)((afterItems.Corrected() - beforeItems.Corrected()) * instrument.CorrectedMultiplier()), 4);
        }

        
        public static decimal? EvcUncorrected(this Instrument instrument, Dictionary<int, string> beforeItems, Dictionary<int, string> afterItems)
        {
            if (afterItems == null || beforeItems == null) return null;

            return Math.Round((decimal)((afterItems.Uncorrected() - beforeItems.Uncorrected()) * instrument.UnCorrectedMultiplier()), 4);
        }

        
        public static decimal? Corrected(this Dictionary<int, string> itemValues)
        {
            return GetHighResolutionValue(itemValues, COR_VOLUME, COR_VOLUME_HIGH_RES);
        }

        
        public static decimal? Uncorrected(this Dictionary<int, string> itemValues)
        {
            return GetHighResolutionValue(itemValues, UNCOR_VOL, UNCOR_VOL_HIGHRES);
        }

        public static string DriveRateDescription(this Instrument instrument)
        {
            return instrument.ItemDetails.GetItem(98).GetDescriptionValue(instrument.ItemValues);
        }

        public static decimal DriveRate(this Instrument instrument)
        {
            return instrument.ItemDetails.GetItem(98).GetNumericValue(instrument.ItemValues);
        }
        
        public static decimal? CorrectedMultiplier(this Instrument instrument)
        {
            return instrument.ItemDetails.GetItem(90).GetNumericValue(instrument.ItemValues);
        }
        
        public static string CorrectedMultiplierDescription(this Instrument instrument)
        {
            return instrument.ItemDetails.GetItem(90).GetDescriptionValue(instrument.ItemValues);
        }

        
        public static decimal? UnCorrectedMultiplier(this Instrument instrument)
        {
            return instrument.ItemDetails.GetItem(92).GetNumericValue(instrument.ItemValues);
        }

        
        public static string UnCorrectedMultiplierDescription(this Instrument instrument)
        {
            return instrument.ItemDetails.GetItem(92).GetDescriptionValue(instrument.ItemValues);
        }
                
      
        public static decimal? GetHighResolutionValue(this Dictionary<int, string> itemValues, int lowResItemNumber, int highResItemNumber)
        {
            if (itemValues == null || itemValues.Count() == 0) return null;

            var lowResValue = itemValues.FirstOrDefault(x => x.Key == lowResItemNumber).Value;
            if (lowResValue == null) return null;

            var highResValue = itemValues.FirstOrDefault(x => x.Key == highResItemNumber).Value;
            if (highResValue == null) return null;

            return JoinLowResHighResReading(decimal.Parse(lowResValue), decimal.Parse(highResValue));
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
