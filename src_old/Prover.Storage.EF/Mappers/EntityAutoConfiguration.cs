using System;
using FluentModelBuilder.Configuration;
using Prover.Shared.Domain;

namespace Prover.Storage.EF.Mappers
{
    public class EntityAutoConfiguration : DefaultEntityAutoConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            var isMap = IsSubclassOfRawGeneric(typeof(Entity), type);
            return base.ShouldMap(type) && isMap;
        }

        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                    return true;
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}