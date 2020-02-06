using System;
using System.Linq;
using System.Security.Cryptography;

namespace Devices.Core.Items
{
    public abstract class ItemValue
    {
        public static ItemValue Create(ItemMetadata metadata, object value)
        {
            if (metadata.ItemDescriptions.Any())
            {
                return new ItemValueWithDescription(metadata, value);
            }

            return new ItemValueBasic(metadata, value);
        }

        protected ItemValue(ItemMetadata metadata, object value)
        {
            RawValue = value;

            var myVal = value.ToString().Trim();

            if (decimal.TryParse(myVal, out var result))
                Value = result;
            else
                Value = myVal;

            Metadata = metadata;
        }

        #region Public Properties

        public object RawValue { get; }
        public object Value { get; protected set; }
        public int Id => Metadata?.Number ?? -1;
        public Type ValueType => Value.GetType();

        public ItemMetadata Metadata { get; }

        public decimal? DecimalValue()
        {
            return decimal.TryParse(Value.ToString(), out var result) ? result : default(decimal?);
        }

        public int? IntValue()
        {
            return int.TryParse(Value.ToString(), out var result) ? result : default(int?);
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return $" {Metadata?.Description} - #{Metadata?.Number} {Environment.NewLine}" +
                   $"   Item Value: {Value} {Environment.NewLine}";
        }

        #endregion
    }

    public class ItemValueBasic : ItemValue
    {
        public ItemValueBasic(ItemMetadata metadata, object value) : base(metadata, value)
        {
        }
    }

    public class ItemValueWithDescription : ItemValue
    {
        public ItemValueWithDescription(ItemMetadata metadata, object value) : base(metadata, value)
        {

            Description = Metadata?.GetItemDescription(Value);

            if (Description != null)
            {
                if (Description.NumericValue != null)
                    Value = Description.NumericValue;
                else
                    Value = Description.Description;
            }
        }

        #region Public Properties

        public ItemMetadata.ItemDescription Description { get; }

        #endregion
    }
}