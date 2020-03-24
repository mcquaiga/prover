using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devices.Core.Interfaces;

namespace Devices.Core.Items.ItemGroups.Builders
{
    public abstract class ItemGroupFactoryBase : ItemGroupFactory
    {
        protected readonly Dictionary<Type, IBuildItemsFor<ItemGroup>> BuildersCache =
            new Dictionary<Type, IBuildItemsFor<ItemGroup>>();

        //private protected readonly DeviceType DeviceType;
        protected ItemGroupBuilderBase<ItemGroup> BasicGroupBuilder;

        protected HashSet<Type> BuilderTypes;
        
        protected ItemGroupFactoryBase()
        {
        }

        protected abstract Assembly BaseAssembly { get; }

        #region ItemGroupFactory Members

        //var builder = findGroupBuilder(typeof(TGroup));
        //if (builder != null)
        //    return (TGroup) builder.Build(DeviceType, values);
        public virtual TGroup Create<TGroup>(DeviceType deviceType, IEnumerable<ItemValue> values) where TGroup : ItemGroup =>
            (TGroup) BasicGroupBuilder.GetItemGroupInstance(typeof(TGroup), values, deviceType);

        public virtual ItemGroup Create(DeviceType deviceType, IEnumerable<ItemValue> values, Type groupType)
        {
            return BasicGroupBuilder.GetItemGroupInstance(groupType, values, deviceType);
        }
        #endregion

        protected IBuildItemsFor<ItemGroup> findGroupBuilder(Type type, DeviceType deviceType)
        {
            loadGroupBuilders(deviceType);

            var groupClass = type.GetMatchingItemGroupClass(deviceType);

            if (groupClass != null)
            {
                if (BuildersCache.ContainsKey(groupClass))
                    return BuildersCache[groupClass];

                var builder = BuilderTypes.FirstOrDefault(
                    t => t.GetInterfaces().Any(
                        y => y.GenericTypeArguments.Contains(groupClass) || y.GenericTypeArguments.Contains(type)));

                if (builder != null)
                {
                    var instance = Activator.CreateInstance(builder) as IBuildItemsFor<ItemGroup>;

                    BuildersCache.Add(groupClass, instance);
                    return instance;
                }
            }
            //var make = ItemGroupBuilderType.MakeGenericType(new[] {groupClass});
            //var x = Activator.CreateInstance(make, DeviceType);
            //var obj = Activator.CreateInstance(make, DeviceType) as ItemGroupBuilder<ItemGroup>;
            //BuildersCache.Add(groupClass, obj);

            return null;
        }

        protected virtual void loadGroupBuilders(DeviceType deviceType)
        {
            if (BuilderTypes == null)
            {
                BuilderTypes = new HashSet<Type>();

                var myType = typeof(IBuildItemsFor<>);

                var builders = Assembly.GetAssembly(deviceType.GetType()).GetTypes().Where(p =>
                    p.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == myType));
                BuilderTypes.UnionWith(builders.ToList());

                if (BaseAssembly != null)
                {
                    builders = BaseAssembly.GetTypes().Where(p =>
                        p.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == myType));
                    BuilderTypes.UnionWith(builders.ToList());
                }
            }
        }
    }
}