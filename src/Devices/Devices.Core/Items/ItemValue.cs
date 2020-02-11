using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Items.Descriptions;

namespace Devices.Core.Items
{
    public class ItemValue
    {
        private ItemValue() { }

        public static ItemValue Create(ItemMetadata metadata, object value)
        {
            if (value.ToString().Equals("! Unsupported"))
            {

            }
            else if (metadata.ItemDescriptions.Any())
            {
                return new ItemValueWithDescription(metadata, value);
            }

            return new ItemValue(metadata, value);
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
        protected object Value { get; set; }
        public int Id => Metadata?.Number ?? -1;
        public Type ValueType => Value.GetType();

        public ItemMetadata Metadata { get; }

        public decimal? DecimalValue()
        {
            return decimal.TryParse(GetValue().ToString(), out var result) ? result : default(decimal?);
        }

        public int? IntValue()
        {
            return int.TryParse(GetValue().ToString(), out var result) ? result : default(int?);
        }

        public virtual object GetValue()
        {
            return Value;
        }

        public virtual string GetDescription()
        {
            return Value.ToString();
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


    public class ItemValueWithDescription : ItemValue
    {
        public ItemValueWithDescription(ItemMetadata metadata, object value) : base(metadata, value)
        {
            ItemDescription = Metadata?.GetItemDescription(Value);

            if (ItemDescription == null)
                throw new ArgumentNullException($"Item #{metadata.Number}: Couldn't find description matching value {Value}");
        }

        public ItemDescription ItemDescription { get; }

        public override object GetValue()
        {
            return ItemDescription.GetValue();
        }

        public override string GetDescription()
        {
            return ItemDescription.Description;
        }
    }

    //public class ItemValueWithDescription44 : ItemValue
    //{
    //    public ItemValueWithDescription44(ItemMetadata metadata, object value) : base(metadata, value)
    //    {
    //        ItemDescription = Metadata?.GetItemDescription(Value);
    //    }

    //    #region Public Properties

    //    public ItemDescription ItemDescription { get; }

    //    public override object GetValue()
    //    {
    //        return ItemDescription.GetValue();
    //    }

    //    public override string GetDescription()
    //    {
    //        return ItemDescription.Description;
    //    }

    //    #endregion
    //}
}