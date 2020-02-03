using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devices.Core.Interfaces.Items;

namespace Devices.Core.Items.Attributes
{
    public static class ItemInfoAttributeHelpers
    {
        public static CustomAttributeData GetByNumber(this IList<CustomAttributeData> attributes, int number)
        {
            var cad = attributes
                .FirstOrDefault(a => a.AttributeType == typeof(ItemInfoAttribute) && (int)a.ConstructorArguments[0].Value == number);

            return cad ?? default;
        }

        public static ICollection<int> GetItemIdentifiers<T>() where T : IItemGroup
        {
            return GetItemIdentifiers(typeof(T));
        }

        public static ICollection<int> GetItemIdentifiers(Type typeGroup)
        {
            var props = typeGroup.GetProperties(BindingFlags.Public | BindingFlags.Instance)
               .Where(p => p.GetCustomAttributes(typeof(ItemInfoAttribute), false).Any())
               .ToList();

            var items = props.Select(p =>
                    p.GetCustomAttributesData().FirstOrDefault(a => a.AttributeType == typeof(ItemInfoAttribute)))
                            .Select(d => (int)d.ConstructorArguments[0].Value);

            return items.ToList();
        }

        public static int GetNumber(this CustomAttributeData attribute)
        {
            if (attribute.AttributeType == typeof(ItemInfoAttribute))
            {
                return (int)attribute.ConstructorArguments[0].Value;
            }

            return -1;
        }

        public static int GetNumber(this PropertyInfo propertyInfo)
        {
            var prop = propertyInfo.GetCustomAttributesData().FirstOrDefault(c => c.AttributeType == typeof(ItemInfoAttribute));
            if (prop != null)
            {
                return (int)prop.ConstructorArguments[0].Value;
            }

            return -1;
        }

        public static IEnumerable<int> GetNumbers(this IList<CustomAttributeData> attributes)
        {
            return attributes
                .Where(a => a.AttributeType == typeof(ItemInfoAttribute))
                .Select(ca => (int)ca.ConstructorArguments[0].Value);
        }
    }
}