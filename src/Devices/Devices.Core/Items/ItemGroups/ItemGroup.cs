using System;
using System.Reflection;
using Devices.Core.Interfaces;
using Devices.Core.Items.Descriptions;

namespace Devices.Core.Items.ItemGroups
{
    public abstract class ItemGroup : IItemGroup
    {
        public virtual void SetValue(PropertyInfo property, ItemValue value)
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
    }
}