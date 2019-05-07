using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using Devices.Honeywell.Core.Attributes;

namespace Devices.Honeywell.Core.ItemGroups
{
    internal abstract class ItemGroupBase : IItemsGroup
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
            decimal? lowResValue = items?.GetItem(lowResItemNumber)?.NumericValue ?? 0;
            decimal? highResValue = items?.GetItem(highResItemNumber)?.NumericValue ?? 0;

            return JoinLowResHighResReading(lowResValue, highResValue);
        }

        public static decimal JoinLowResHighResReading(decimal? lowResValue, decimal? highResValue)
        {
            if (!lowResValue.HasValue || !highResValue.HasValue)
                throw new ArgumentNullException(nameof(lowResValue) + " & " + nameof(highResValue));

            return GetHighResolutionItemValue((int)lowResValue.Value, highResValue.Value);
        }

        public virtual void SetValues(IEnumerable<ItemValue> values)
        {
            values.Join(GetType().GetProperties(),
                       x => x.Id,
                       y => y.GetNumber(),
                       (x, y) => new Tuple<ItemValue, PropertyInfo>(x, y))
               .ToList()
               .ForEach(pair =>
               {
                   SetValue(pair.Item2, pair.Item1);
               });
        }

        protected virtual void SetValue(PropertyInfo property, ItemValue value)
        {
            if (property.PropertyType.IsEnum)
            {
                var v = value.Description;
                var e = Enum.Parse(property.PropertyType, v);

                property.SetValue(this, e);
            }
            else if (property.PropertyType == typeof(decimal))
            {
                property.SetValue(this, value.NumericValue);
            }
            else if (property.PropertyType == typeof(int))
            {
                property.SetValue(this, (int)value.NumericValue);
            }
            else if (property.PropertyType == typeof(string))
            {
                property.SetValue(this, value.Description);
            }
            else
            {
                throw new NotImplementedException($"{property.PropertyType} is not yet supported.");
            }
        }
    }
}