using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;

namespace Devices.Core.Items.ItemGroups.Builders
{
    public abstract class ItemGroupFactoryBase : IItemGroupFactory
    {
        protected readonly Dictionary<Type, IBuildItemsFor<IItemGroup>> BuildersCache =
            new Dictionary<Type, IBuildItemsFor<IItemGroup>>();

        protected readonly DeviceType DeviceType;
        protected ItemGroupBuilderBase<IItemGroup> BasicGroupBuilder;

        protected HashSet<Type> BuilderTypes;

        protected ItemGroupFactoryBase(DeviceType deviceType)
        {
            DeviceType = deviceType;
        }

        #region Public Methods

        public virtual TGroup Create<TGroup>(IEnumerable<ItemValue> values) where TGroup : IItemGroup
        {
            var builder = findGroupBuilder(typeof(TGroup));
            if (builder != null)
                return (TGroup) builder.Build(DeviceType, values);

            return (TGroup) BasicGroupBuilder.GetItemGroupInstance(typeof(TGroup), values);
        }

        #endregion

        #region Protected

        protected abstract Assembly BaseAssembly { get; }

        protected IBuildItemsFor<IItemGroup> findGroupBuilder(Type type)
        {
            loadGroupBuilders();

            var groupClass = type.GetMatchingItemGroupClass(Assembly.GetAssembly(typeof(Type)), BaseAssembly);

            if (groupClass != null)
            {
                if (BuildersCache.ContainsKey(groupClass))
                    return BuildersCache[groupClass];

                var builder = BuilderTypes.FirstOrDefault(
                    t => t.GetInterfaces().Any(
                        y => y.GenericTypeArguments.Contains(groupClass) || y.GenericTypeArguments.Contains(type)));

                if (builder != null)
                {
                    var instance = Activator.CreateInstance(builder) as IBuildItemsFor<IItemGroup>;

                    BuildersCache.Add(groupClass, instance);
                    return instance;
                }
            }
            //var make = ItemGroupBuilderType.MakeGenericType(new[] {groupClass});
            //var x = Activator.CreateInstance(make, DeviceType);
            //var obj = Activator.CreateInstance(make, DeviceType) as IItemGroupBuilder<IItemGroup>;
            //BuildersCache.Add(groupClass, obj);

            return null;
        }

        protected virtual void loadGroupBuilders()
        {
            if (BuilderTypes == null)
            {
                BuilderTypes = new HashSet<Type>();

                var myType = typeof(IBuildItemsFor<>);

                var builders = Assembly.GetAssembly(this.GetType()).GetTypes().Where(p =>
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

        #endregion
    }
}