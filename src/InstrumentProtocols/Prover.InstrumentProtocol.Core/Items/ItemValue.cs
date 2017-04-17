using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;

namespace Prover.InstrumentProtocol.Core.Items
{
    public class ItemValue
    {
        public ItemValue(ItemMetadata metadata, string value)
        {
            RawValue = value;
            Metadata = metadata;
        }

        public virtual string Description
            => ItemDescription?.Description ?? NumericValue.ToString(CultureInfo.InvariantCulture);

        public ItemMetadata Metadata { get; }

        public virtual double NumericValue
            => RawValue != "!Unsupported" ? ItemDescription?.Value ?? double.Parse(RawValue) : 0;

        public string RawValue { get; set; }

        private ItemMetadata.ItemDescription ItemDescription
        {
            get
            {
                if (Metadata?.ItemDescriptions == null || !Metadata.ItemDescriptions.Any()) return null;

                var intValue = Convert.ToInt32(RawValue);
                return Metadata.ItemDescriptions.FirstOrDefault(x => x.Id == intValue);
            }
        }

        public override string ToString()
        {
            return $" {Metadata?.LongDescription} - #{Metadata?.Number} {Environment.NewLine}" +
                   $"   Item Value: {RawValue} {Environment.NewLine}" +
                   $"   Item Description: {Description} {Environment.NewLine}" +
                   $"   Numeric Value: {NumericValue} {Environment.NewLine}";
        }
    }

    public static class ItemValueExtensions
    {
        public static ItemValue GetItem(this IEnumerable<ItemValue> items, string code)
        {
            var result = items.FirstOrDefault(x => x.Metadata.Code.ToLower() == code.ToLower());
            //if (result == null) NLog.LogManager.GetCurrentClassLogger().Warn($"Item code {code} could not be found.");

            return result;
        }

        public static ItemValue GetItem(this IEnumerable<ItemValue> items, int itemNumber)
        {
            var result = items.FirstOrDefault(x => x.Metadata.Number == itemNumber);

            //if (result == null) NLog.LogManager.GetCurrentClassLogger().Warn($"Item number {itemNumber} could not be found.");         

            return result;
        }

        public static string Serialize(this IEnumerable<ItemValue> items)
        {
            if (items == null) return string.Empty;
            return JsonConvert.SerializeObject(items.ToDictionary());
        }

        public static Dictionary<int, string> ToDictionary(this IEnumerable<ItemValue> items)
        {
            if (items == null) return new Dictionary<int, string>();
            return items.ToDictionary(k => k.Metadata.Number, v => v.RawValue);
        }
    }
}