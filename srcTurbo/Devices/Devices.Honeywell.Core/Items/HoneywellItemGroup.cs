using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using Devices.Core.Items.Attributes;
using Devices.Core.Items.ItemGroups;

namespace Devices.Honeywell.Core.Items
{
    public abstract class HoneywellItemGroup : ItemGroup
    {
        #region Public Methods

        public override void SetValues(IEnumerable<ItemValue> values)
        {
            values.Join(
                    GetType().GetProperties().Where(p => p.GetCustomAttribute(typeof(ItemInfoAttribute)) != null),
                    x => x.Id,
                    y => y.GetNumber(),
                    (x, y) => new Tuple<ItemValue, PropertyInfo>(x, y))
                .ToObservable()
                .Subscribe(pair => SetPropertyValue(pair.Item2, pair.Item1));
        }

        #endregion

        #region Protected

        protected void SetPropertyValue(PropertyInfo property, ItemValue value)
        {
            var valueString = value.Value.ToString();

            if (property.PropertyType.IsEnum)
            {
                var e = Enum.Parse(property.PropertyType, valueString);
                property.SetValue(this, e);
            }
            else if (property.PropertyType == typeof(decimal))
            {
                property.SetValue(this, decimal.Parse(valueString));
            }
            else if (property.PropertyType == typeof(int))
            {
                property.SetValue(this, int.Parse(valueString));
            }
            else if (property.PropertyType == typeof(string))
            {
                property.SetValue(this, valueString);
            }
            else if (property.PropertyType.IsSubclassOf(typeof(ItemMetadata.ItemDescription)))
            {
                property.SetValue(this, (value as ItemValueWithDescription)?.Description);
            }
            else
            {
                throw new NotImplementedException($"{property.PropertyType} is not yet supported.");
            }
        }

        #endregion
    }
}