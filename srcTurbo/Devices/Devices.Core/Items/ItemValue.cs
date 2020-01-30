using System;
using System.Globalization;

namespace Devices.Core.Items
{
    public class ItemValue
    {
        //public ItemValue(ItemMetadata metadata, string value)
        //{
        //    RawValue = value;
        //    Metadata = metadata;
        //}

        public ItemValue(ItemMetadata metadata, object value)
        {
            Value = value;
            Metadata = metadata;
        }

        public object Value { get; }

        public int Id => Metadata != null ? Metadata.Number : -1;
        public bool IsInteger => Value is int || Value is uint || Value is ulong;

        public bool IsDecimal => Value is decimal || Value is double || Value is float;

        public ItemMetadata Metadata { get; }

        public virtual decimal NumericValue
        {
            get
            {
                //if (!decimal.TryParse(Value, out var result)) return 0;
                if (Value == null) return 0;

                return ItemDescription?.NumericValue ?? decimal.Parse(Value.ToString());
            }
        }

        //public string RawValue { get; set; }

        public virtual string Description
            => ItemDescription?.Description ?? NumericValue.ToString(CultureInfo.InvariantCulture);

        public ItemMetadata.ItemDescription ItemDescription => Metadata?.GetItemDescription(Value);

        public override string ToString()
        {
            return $" {Metadata?.Description} - #{Metadata?.Number} {Environment.NewLine}" +
                   $"   Item Value: {Value} {Environment.NewLine}" +
                   $"   Item Description: {Description} {Environment.NewLine}" +
                   $"   Numeric Value: {NumericValue} {Environment.NewLine}";
        }
    }
}