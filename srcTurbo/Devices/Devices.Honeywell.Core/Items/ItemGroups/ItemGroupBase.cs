using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using Devices.Honeywell.Core.Attributes;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    public abstract class ItemGroupBase : IItemsGroup
    {
        public virtual void SetValues(IEnumerable<ItemValue> values)
        {
            values.Join(GetType().GetProperties().Where(p => p.GetCustomAttribute(typeof(ItemInfoAttribute)) != null),
                       x => x.Id,
                       y => y.GetNumber(),
                       (x, y) => new Tuple<ItemValue, PropertyInfo>(x, y))
               .ToList()
               .ForEach(pair =>
               {
                   SetValue(pair.Item2, pair.Item1);
               });

            //values.Join(GetType().GetProperties().Where(p => Attribute.GetCustomAttribute(p.GetType(), typeof(JoinLowResHighResValueAttribute)) != null),
            //    x => x.Id,
            //    y => (Attribute.GetCustomAttribute(y.GetType(), typeof(JoinLowResHighResValueAttribute)) as JoinLowResHighResValueAttribute).FirstItem,
            //    (x, y) =>
            //    {
            //        var secondValue = values.GetItem(y..SecondItem);
            //        var result = y.ResultSelector.Invoke(x.NumericValue, secondValue.NumericValue);
            //        return new Tuple<decimal, PropertyInfo>(result, y.)
            //    })
            //    .ToList()
        }

        protected virtual void SetValue(PropertyInfo property, ItemValue value)
        {
            if (property.PropertyType.IsEnum)
            {
                var v = value.Description;
                var e = Enum.Parse(property.PropertyType, v);

                property.SetValue(this, e);
            }
            else if (property.PropertyType == typeof(decimal))
            {
                property.SetValue(this, value.NumericValue);
            }
            else if (property.PropertyType == typeof(int))
            {
                property.SetValue(this, (int)value.NumericValue);
            }
            else if (property.PropertyType == typeof(string))
            {
                property.SetValue(this, value.Description);
            }
            else
            {
                throw new NotImplementedException($"{property.PropertyType} is not yet supported.");
            }
        }
    }
}