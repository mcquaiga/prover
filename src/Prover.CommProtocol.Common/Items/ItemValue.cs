using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Prover.CommProtocol.Common.Items
{
    public class ItemValue
    {
        public ItemValue(ItemMetadata metadata, string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

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
                if (RawValue.Contains("Unsupported"))
                    return 0;

                return ItemDescription?.Value ?? decimal.Parse(RawValue);
            }
        }

        public virtual string Description => ItemDescription?.Description ?? "[NULL]";

        private ItemMetadata.ItemDescription ItemDescription
        {
            get
            {
                if ((Metadata?.ItemDescriptions != null) && Metadata.ItemDescriptions.Any())
                {
                    var intValue = Convert.ToInt32(RawValue);
                    return Metadata.ItemDescriptions.FirstOrDefault(x => x.Id == intValue);
                }

                return null;
            }
        }

        public override string ToString()
        {
            return $"{Environment.NewLine}================================================={Environment.NewLine}" +
                   $"{Metadata?.LongDescription} - #{Metadata?.Number} {Environment.NewLine}" +
                   $"   Item Value: {RawValue} {Environment.NewLine}" +
                   $"   Item Description: {Description} {Environment.NewLine}" +
                   $"   Numeric Value: {NumericValue} {Environment.NewLine}";
        }
    }

    public static class ItemValueExtensions
    {
        public static ItemValue GetItem(this IEnumerable<ItemValue> items, string code)
        {
            var result = items?.FirstOrDefault(x => x.Metadata?.Code.ToLower() == code.ToLower());
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