using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Items.Descriptions;

namespace Devices.Core.Items
{
    public class ItemValue : IEquatable<ItemValue>//, IEqualityComparer
    {
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

        public ItemValue()
        {

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

        public bool Equals(ItemValue other) => Metadata.Number == other?.Metadata.Number;

        public override string ToString()
        {
            return $"{Metadata?.Number} - {Metadata?.Description} = {Value}";
        }

        #endregion

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as ItemValue);

        /// <inheritdoc />
        public override int GetHashCode() => (Metadata.Number).GetHashCode();

    
      

        /// <inheritdoc />
        //public bool Equals(object x, object y) => Equals(x, y);

        /// <inheritdoc />
        //public int GetHashCode(object obj) => throw new NotImplementedException();
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

 
}