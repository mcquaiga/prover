using System;
using System.Collections.Generic;
using System.Reflection;

namespace Devices.Communications
{
    internal static class TypeFindingHelpers
    {
        public static IEnumerable<Type> GetAllTypesImplementingOpenGenericType(Type openGenericType, Assembly[] assemblies)
        {
            throw new NotImplementedException(nameof(GetAllTypesImplementingOpenGenericType));
            //var types = assemblies.Where(a => a.FullName.Contains("Devices."))
            //    .SelectMany(ass => GetDerivedTypes(openGenericType, ass));

            //var t1 = types.SelectMany(y => y.GetInterfaces().Where(i => i.IsGenericType))
            //    .Where(t => t != null && t.IsGenericType && openGenericType.IsAssignableFrom(t.GetGenericTypeDefinition()));

            //var t2 = types.Select(t => t.BaseType)
            //    .Where(z => z != null && z.IsGenericType && openGenericType.IsAssignableFrom(z.GetGenericTypeDefinition()));

            //return t1.Concat(t2).ToList();
            //return from ass in assemblies
            //       where
            //       from x in ass.GetTypes()
            //       from z in x.GetInterfaces()
            //       let y = x.BaseType
            //       where (y != null && y.IsGenericType && openGenericType.IsAssignableFrom(y.GetGenericTypeDefinition()))
            //            || (z.IsGenericType && openGenericType.IsAssignableFrom(z.GetGenericTypeDefinition()))
            //       select x;
        }

        public static List<Type> GetDerivedTypes(Type baseType, Assembly assembly)
        {
            // Get all types from the given assembly
            Type[] types = assembly.GetTypes();
            List<Type> derivedTypes = new List<Type>();

            for (int i = 0, count = types.Length; i < count; i++)
            {
                Type type = types[i];
                if (IsSubclassOf(type, baseType))
                {
                    // The current type is derived from the base type,
                    // so add it to the list
                    derivedTypes.Add(type);
                }
            }

            return derivedTypes;
        }

        public static bool IsSubclassOf(Type type, Type baseType)
        {
            if (type == null || baseType == null || type == baseType)
                return false;

            if (baseType.IsGenericType == false)
            {
                if (type.IsGenericType == false)
                    return type.IsSubclassOf(baseType);
            }
            else
            {
                baseType = baseType.GetGenericTypeDefinition();
            }

            type = type.BaseType;
            Type objectType = typeof(object);

            while (type != objectType && type != null)
            {
                Type curentType = type.IsGenericType ?
                    type.GetGenericTypeDefinition() : type;
                if (curentType == baseType)
                    return true;

                type = type.BaseType;
            }

            return false;
        }
    }
}