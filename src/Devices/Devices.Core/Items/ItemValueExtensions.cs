using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Items.Descriptions;
using Newtonsoft.Json;

namespace Devices.Core.Items
{
    public static class ItemValueExtensions
    {
        public static ItemValue GetItem(this IEnumerable<ItemValue> items, string code)
        {
            var result = items?.FirstOrDefault(x => x.Metadata?.Code?.ToLower() == code.ToLower());
            //if (result == null) NLog.LogManager.GetCurrentClassLogger().Warn($"Item code {code} could not be found.");

            return result;
        }

        public static ItemValue GetItem(this IEnumerable<ItemValue> items, int itemNumber)
        {
            var result = items?.FirstOrDefault(x => x.Metadata?.Number == itemNumber);

            //if (result == null) NLog.LogManager.GetCurrentClassLogger().Warn($"Item number {itemNumber} could not be found.");

            return result;
        }

        public static TItem GetItem<TItem>(this IEnumerable<ItemValue> items, int itemNumber) where TItem : ItemValue
        {
            return (TItem) GetItem(items, itemNumber);
        }

        public static decimal GetItemValue(this IEnumerable<ItemValue> items, int itemNumber)
        {
            var i = GetItem(items, itemNumber);
            var x = i?.DecimalValue();
            
            if (!x.HasValue) 
                throw new NullReferenceException($"Item #{itemNumber} has a null value.");

            return x.Value;
        }

        public static decimal? GetItemValueNullable(this IEnumerable<ItemValue> items, int itemNumber)
        {
            return GetItem(items, itemNumber)?.DecimalValue();
        }

        public static ItemDescription GetItemDescription(this IEnumerable<ItemValue> items, int itemNumber)
        {
            return (GetItem(items, itemNumber) as ItemValueWithDescription)?.ItemDescription;
        }

        public static decimal? GetValue(this ItemValue value)
        {
            return value.DecimalValue();
        }

        //public static decimal GetValue(this ItemValueWithDescription value)
        //{
        //    if (decimal.TryParse(value.ItemDescription.GetValue(), ))
        //        return value.ItemDescription.Id;

        //    return value.ItemDescription.NumericValue.Value;
        //}

        public static string GetDescription(this ItemValueWithDescription value)
        {
            return value.ItemDescription.Description;
        }

        public static string Serialize(this IEnumerable<ItemValue> items)
        {
            if (items == null) return string.Empty;
            return JsonConvert.SerializeObject(items.ToItemValuesDictionary());
        }

        public static Dictionary<int, string> ToItemValuesDictionary(this IEnumerable<ItemValue> items)
        {
            try
            {
                if (items == null) return new Dictionary<int, string>();
                return items
                    .Where(i => i.Metadata != null)
                    .ToDictionary(k => k.Metadata.Number, v => v.RawValue.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}