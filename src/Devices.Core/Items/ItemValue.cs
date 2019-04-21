using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Devices.Core.Items
{
    public static class ItemValueExtensions
    {
        #region Public Methods

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

        public static string Serialize(this IEnumerable<ItemValue> items)
        {
            if (items == null) return string.Empty;
            return JsonConvert.SerializeObject(items.ToDictionary());
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

        #endregion Public Methods
    }

    public class ItemValue
    {
        #region Public Constructors

        public ItemValue(ItemMetadata metadata, string value)
        {
            RawValue = value;
            Metadata = metadata;
        }

        #endregion Public Constructors

        #region Public Properties

        public virtual string Description
            => ItemDescription?.Description ?? NumericValue.ToString(CultureInfo.InvariantCulture);

        public ItemMetadata.ItemDescription ItemDescription
        {
            get { return Metadata?.GetItemDescription(RawValue); }
        }

        public ItemMetadata Metadata { get; }

        public virtual decimal NumericValue
        {
            get
            {
                if (!decimal.TryParse(RawValue, out var result)) return 0;

                return ItemDescription?.NumericValue ?? result;
            }
        }

        public string RawValue { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            return $" {Metadata?.Description} - #{Metadata?.Number} {Environment.NewLine}" +
                   $"   Item Value: {RawValue} {Environment.NewLine}" +
                   $"   Item Description: {Description} {Environment.NewLine}" +
                   $"   Numeric Value: {NumericValue} {Environment.NewLine}";
        }

        #endregion Public Methods
    }
}