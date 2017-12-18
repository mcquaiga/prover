using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prover.CommProtocol.Common.Items;

namespace Prover.CommProtocol.MiHoneywell.Items
{
    public abstract class DeviceItems
    {
        [JsonConstructor]
        protected DeviceItems()
        {
        }

        public static decimal GetHighResolutionValue(IEnumerable<ItemValue> itemValues, int lowResItemNumber,
            int highResItemNumber)
        {
            if (itemValues == null) return 0m;

            var items = itemValues as ItemValue[] ?? itemValues.ToArray();
            decimal? lowResValue = items?.GetItem(lowResItemNumber)?.NumericValue ?? 0;
            decimal? highResValue = items?.GetItem(highResItemNumber)?.NumericValue ?? 0;

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

            return GetHighResolutionItemValue((int)lowResValue.Value, highResValue.Value);
        }
    }

    public class FrequencyTestItems : DeviceItems, IFrequencyTestItems
    {    
        [JsonConstructor]
        private FrequencyTestItems() { }

        public FrequencyTestItems(IEnumerable<ItemValue> itemValues)
        {
            var items = itemValues.ToList();

            AdjustedVolumeReading = GetHighResolutionValue(items, 850, 851);
            UnadjustVolumeReading = (long)items.GetItem(852).NumericValue;
        }

        public decimal AdjustedVolumeReading { get; set; }
        public long UnadjustVolumeReading { get; set; }
    }
}
