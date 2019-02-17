using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;

namespace Prover.CommProtocol.Common.Items
{
    public class ItemValue
    {
        public ItemValue(ItemMetadata metadata, string value)
        {
            //if (string.IsNullOrEmpty(value))
            //    throw new ArgumentNullException(nameof(value));

            //if (metadata == null)
            //    throw new ArgumentNullException(nameof(metadata));

            RawValue = value;
            Metadata = metadata;
        }

        public string RawValue { get; set; }
        public ItemMetadata Metadata { get; }

        public virtual decimal NumericValue        
        {
            get
            {
                if (!decimal.TryParse(RawValue, out var result)) return 0;

                return ItemDescription?.NumericValue ?? result;
            }
        }

        public virtual string Description 
            => ItemDescription?.Description ?? NumericValue.ToString(CultureInfo.InvariantCulture);

        private ItemMetadata.ItemDescription ItemDescription
        {
            get { return Metadata?.GetItemDescription(RawValue); }
        }

        public override string ToString()
        {
            return $" {Metadata?.Description} - #{Metadata?.Number} {Environment.NewLine}" +
                   $"   Item Value: {RawValue} {Environment.NewLine}" +
                   $"   Item Description: {Description} {Environment.NewLine}" +
                   $"   Numeric Value: {NumericValue} {Environment.NewLine}";
        }
    }

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

        public static Dictionary<int, string> ToDictionary(this IEnumerable<ItemValue> items)
        {
            try
            {
                if (items == null) return new Dictionary<int, string>();
                return items
                    .Where(i => i.Metadata != null)
                    .ToDictionary(k => k.Metadata.Number, v => v.RawValue);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }

        public static string Serialize(this IEnumerable<ItemValue> items)
        {
            if (items == null) return string.Empty;
            return JsonConvert.SerializeObject(items.ToDictionary());
        }
    }
}