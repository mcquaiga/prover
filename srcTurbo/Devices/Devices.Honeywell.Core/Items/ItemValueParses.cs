using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Devices.Core.Items;

namespace Devices.Honeywell.Core.Items
{
    public static class ItemValueParses
    {
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

        public static decimal GetHighResolutionValue(IEnumerable<ItemValue> itemValues, int lowResItemNumber,
            int highResItemNumber)
        {
            if (itemValues == null) return 0m;

            var items = itemValues as ItemValue[] ?? itemValues.ToArray();
            var lowResValue = items?.GetItem(lowResItemNumber)?.DecimalValue() ?? 0;
            var highResValue = items?.GetItem(highResItemNumber)?.DecimalValue() ?? 0;

            return JoinLowResHighResReading(lowResValue, highResValue);
        }

        public static decimal JoinLowResHighResReading(decimal? lowResValue, decimal? highResValue)
        {
            if (!lowResValue.HasValue || !highResValue.HasValue)
                throw new ArgumentNullException(nameof(lowResValue) + " & " + nameof(highResValue));

            return GetHighResolutionItemValue((int)lowResValue.Value, highResValue.Value);
        }
    }
}