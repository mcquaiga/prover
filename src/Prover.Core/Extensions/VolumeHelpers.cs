using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Models.Instruments;

namespace Prover.Core.Extensions
{
    public static class VolumeHelpers
    {
        private const int COR_VOLUME = 0;
        private const int COR_VOLUME_HIGH_RES = 113;
        private const int UNCOR_VOL = 2;
        private const int UNCOR_VOL_HIGHRES = 892;
        private const int PULSER_A = 93;
        private const int PULSER_B = 94;
        
        public static string PulseASelect(this Instrument instrument) => instrument.Items.GetItem(PULSER_A).Description;

        public static string PulseBSelect(this Instrument instrument) => instrument.Items.GetItem(PULSER_B).Description;

        public static decimal? EvcCorrected(this Instrument instrument, IEnumerable<ItemValue> beforeItems, IEnumerable<ItemValue> afterItems)
        {
            var o = (afterItems?.Corrected() - beforeItems?.Corrected())*instrument?.CorrectedMultiplier();
            if (o != null)
                return Math.Round((decimal) o, 4);

            return null;
        }


        public static decimal? EvcUncorrected(this Instrument instrument, IEnumerable<ItemValue> beforeItems, IEnumerable<ItemValue> afterItems)
        {
            var o = (afterItems?.Uncorrected() - beforeItems?.Uncorrected())*instrument?.UnCorrectedMultiplier();
            if (o != null)
                return Math.Round(
                    (decimal)
                        o, 4);
            return null;
        }

        public static decimal? Corrected(this IEnumerable<ItemValue> itemValues)
            => GetHighResolutionValue(itemValues, COR_VOLUME, COR_VOLUME_HIGH_RES);

        public static decimal? Uncorrected(this IEnumerable<ItemValue> itemValues)
            => GetHighResolutionValue(itemValues, UNCOR_VOL, UNCOR_VOL_HIGHRES);

        public static string DriveRateDescription(this Instrument instrument)
            => instrument.Items.GetItem(98).Description;

        public static decimal DriveRate(this Instrument instrument) => instrument.Items.GetItem(98).NumericValue;

        public static decimal? CorrectedMultiplier(this Instrument instrument)
            => instrument.Items.GetItem(90).NumericValue;

        public static string CorrectedMultiplierDescription(this Instrument instrument)
            => instrument.Items.GetItem(90).Description;


        public static decimal? UnCorrectedMultiplier(this Instrument instrument)
            => instrument.Items.GetItem(92).NumericValue;


        public static string UnCorrectedMultiplierDescription(this Instrument instrument)
            => instrument.Items.GetItem(92).Description;


        public static decimal? GetHighResolutionValue(this IEnumerable<ItemValue> itemValues, int lowResItemNumber,
            int highResItemNumber)
        {
            if (itemValues == null) return null;

            var lowResValue = itemValues?.GetItem(lowResItemNumber).NumericValue;
            var highResValue = itemValues?.GetItem(highResItemNumber).NumericValue;

            return JoinLowResHighResReading(lowResValue, highResValue);
        }

        public static decimal GetHighResFractionalValue(decimal highResValue)
        {
            if (highResValue == 0) return 0;

            var highResString = Convert.ToString(highResValue, CultureInfo.InvariantCulture);
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

            return GetHighResolutionItemValue((int) lowResValue.Value, highResValue.Value);
        }
    }
}