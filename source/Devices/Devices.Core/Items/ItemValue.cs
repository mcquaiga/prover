using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Items.Descriptions;

namespace Devices.Core.Items
{
    public class ItemValue : IEquatable<ItemValue>, IEqualityComparer<ItemValue>
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
            Metadata = metadata;
            SetValue(value);
        }

        #region Public Properties

        public object RawValue { get; protected set; }
        protected object Value { get; private set; }
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

        public void SetValue(object value)
        {
            RawValue = value;

            var myVal = value.ToString().Trim();

            if (decimal.TryParse(myVal, out var result))
                Value = result;
            else
                Value = myVal;
        }

        #endregion

        #region Public Methods

        public bool Equals(ItemValue other) => Id == other?.Id;

        public override string ToString()
        {
            return $" {Metadata?.Description} - #{Metadata?.Number} {Environment.NewLine}" +
                   $"   Item Value: {Value} {Environment.NewLine}";
        }

        #endregion


        public bool Equals(ItemValue x, ItemValue y)
        {
            return x?.Id == y?.Id;
        }

        public int GetHashCode(ItemValue obj)
        {
            return obj.Id.GetHashCode();
        }
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