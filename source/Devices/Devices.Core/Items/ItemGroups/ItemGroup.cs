using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devices.Core.Interfaces;
using Devices.Core.Items.Attributes;
using Devices.Core.Items.Descriptions;

namespace Devices.Core.Items.ItemGroups
{
    public interface IItemGroup
    {
        void SetPropertyValue(PropertyInfo property, ItemValue value);
        ItemGroup SetValues(DeviceType deviceType, IEnumerable<ItemValue> itemValues);
    }

    public abstract class ItemGroup : IItemGroup
    {
        protected ItemGroup() { }

        public virtual void SetPropertyValue(PropertyInfo property, ItemValue value)
        {
            var valueString = value.GetValue().ToString();

            if (property.PropertyType.IsSubclassOf(typeof(ItemDescription)))
            {
                if (value is ItemValueWithDescription itemDescription)
                    property.SetValue(this, itemDescription.ItemDescription);
            }
            else if (property.PropertyType.IsEnum)
            {
                var e = Enum.Parse(property.PropertyType, valueString);
                property.SetValue(this, e);
            }
            else if (property.PropertyType == typeof(decimal))
            {
                if (decimal.TryParse(valueString, out var d))
                    property.SetValue(this, d);
            }
            else if (property.PropertyType == typeof(int))
            {
                if (!int.TryParse(valueString, out var i))
                {
                    if (decimal.TryParse(valueString, out var d))
                        i = decimal.ToInt32(d);
                    else
                        throw new ArgumentException(
                            $"Value for property {property.Name} with type {property.PropertyType} is the wrong data type.");
                }

                property.SetValue(this, i);
            }
            else if (property.PropertyType == typeof(string))
            {
                property.SetValue(this, valueString);
            }
            else
            {
                throw new NotImplementedException($"{property.PropertyType} is not yet supported.");
            }
        }

        public virtual ItemGroup SetValues(DeviceType deviceType, IEnumerable<ItemValue> itemValues)
        {
            MatchItemValuesWithPropertyInfo(itemValues).ToList()
                .ForEach(pair => SetPropertyValue(pair.Item1, pair.Item2));

            return this;
        }

        protected IEnumerable<Tuple<PropertyInfo, ItemValue>> MatchItemValuesWithPropertyInfo(IEnumerable<ItemValue> values)
        {
            return from value in values
                join prop in ItemInfoAttributes(this.GetType()) on value.Id equals prop.GetNumber()
                select new Tuple<PropertyInfo, ItemValue>(prop, value);
        }

        protected virtual IEnumerable<PropertyInfo> ItemInfoAttributes(Type groupType)
        {
            return groupType.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(Attributes.ItemInfoAttribute)) != null);
        }
    }
}