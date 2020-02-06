using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items.Attributes;

namespace Devices.Core.Items.ItemGroups.Builders
{
    public abstract class ItemGroupBuilderBase<TGroup> where TGroup : IItemGroup
    {
        protected DeviceType DeviceType;

        protected ItemGroupBuilderBase(DeviceType deviceType)
        {
            DeviceType = deviceType;
        }

        public virtual TGroup Build<T>(DeviceType device, IEnumerable<ItemValue> values) where T : IItemGroup
        {
            return GetItemGroupInstance(typeof(T), values);
        }

        public virtual TGroup Build(DeviceType device, IEnumerable<ItemValue> values)
        {
            return GetItemGroupInstance(typeof(TGroup), values);
        }

        //protected abstract Assembly ThisAssembly { get; }

        protected virtual IItemGroup GetItemGroupInstance(Type groupType)
        {
            var itemType = groupType.GetMatchingItemGroupClass();
            if (itemType == null)
                throw new Exception($"Type {groupType.Name} could not be found.");

            var itemGroup = (ItemGroup) Activator.CreateInstance(itemType);

            return itemGroup;
        }

        protected virtual TGroup GetItemGroupInstance(Type type, IEnumerable<ItemValue> itemValues)
        {
            var itemGroup = GetItemGroupInstance(type);
            return SetValues((TGroup) itemGroup, itemValues);
        }

        protected virtual IEnumerable<PropertyInfo> ItemInfoAttributes(Type groupType)
        {
            return groupType.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(Attributes.ItemInfoAttribute)) != null);
        }

        protected IEnumerable<Tuple<PropertyInfo, ItemValue>> MatchItemValuesWithPropertyInfo(Type groupType, IEnumerable<ItemValue> values)
        {
            return (from value in values
                join prop in ItemInfoAttributes(groupType) on value.Id equals prop.GetNumber()
                select new Tuple<PropertyInfo, ItemValue>(prop, value));
        }

        protected virtual PropertyInfo SetPropertyValue(TGroup itemGroup, PropertyInfo property, string valueString)
        {
            if (property.PropertyType.IsEnum)
            {
                var e = Enum.Parse(property.PropertyType, valueString);
                property.SetValue(itemGroup, e);
            }
            else if (property.PropertyType == typeof(decimal))
            {
                if (decimal.TryParse(valueString, out var d))
                    property.SetValue(itemGroup, d);
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
                property.SetValue(itemGroup, i);
            }
            else if (property.PropertyType == typeof(string))
            {
                property.SetValue(itemGroup, valueString);
            }
            else
            {
                throw new NotImplementedException($"{property.PropertyType} is not yet supported.");
            }

            return property;
        }

        protected virtual PropertyInfo SetPropertyValue(TGroup itemGroup, PropertyInfo property, ItemValue value)
        {
            if (property.PropertyType.IsSubclassOf(typeof(ItemMetadata.ItemDescription)))
            {
                property.SetValue(itemGroup, (value as ItemValueWithDescription)?.Description);
            }
            else
            {
                return SetPropertyValue(itemGroup, property, value.Value.ToString());
            }

            return property;
        }

        protected virtual TGroup SetValues(TGroup itemGroup, IEnumerable<ItemValue> itemValues)
        {
            MatchItemValuesWithPropertyInfo(itemGroup.GetType(), itemValues).ToList()
                .ForEach(pair =>
                {
                    var (prop, item) = pair;
                    SetPropertyValue(itemGroup, prop, item);
                });

            return itemGroup;
        }
    }
}