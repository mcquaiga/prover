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
        protected readonly Dictionary<Type, IItemGroupBuilder<IItemGroup>> BuildersCache =
            new Dictionary<Type, IItemGroupBuilder<IItemGroup>>();

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
            return (TGroup) builder.Build<TGroup>(DeviceType, values);
        }

        #endregion

        #region Protected

        protected abstract Assembly BaseAssembly { get; }

        //protected abstract Assembly ThisAssembly { get; }


        protected IItemGroupBuilder<IItemGroup> findGroupBuilder(Type type)
        {
            loadGroupBuilders();

            var groupClass = type.GetMatchingItemGroupClass(BaseAssembly);

            if (groupClass != null)
            {
                if (BuildersCache.ContainsKey(groupClass))
                    return BuildersCache[groupClass];

                var builder = BuilderTypes.FirstOrDefault(
                    t => t.GetInterfaces().Any(
                        y => y.GenericTypeArguments.Contains(groupClass) || y.GenericTypeArguments.Contains(type)));

                if (builder != null)
                {
                    var instance = Activator.CreateInstance(builder, DeviceType) as IItemGroupBuilder<IItemGroup>;

                    BuildersCache.Add(groupClass, instance);
                    return instance;
                }
            }
            //var make = ItemGroupBuilderType.MakeGenericType(new[] {groupClass});
            //var x = Activator.CreateInstance(make, DeviceType);
            //var obj = Activator.CreateInstance(make, DeviceType) as IItemGroupBuilder<IItemGroup>;
            //BuildersCache.Add(groupClass, obj);

            return (IItemGroupBuilder<IItemGroup>) BasicGroupBuilder;
        }

        protected virtual void loadGroupBuilders()
        {
            if (BuilderTypes == null)
            {
                BuilderTypes = new HashSet<Type>();

                var type = typeof(IBuildItemsFor<>);

                var iBuilders = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => type.IsAssignableFrom(p));

                //var localBuilders = Assembly.GetCallingAssembly().GetTypes().Where(x => x.Name.Contains("Builder"));
                //var baseBuilders = BaseAssembly.GetTypes().Where(x => x.Name.Contains("Builder"));

                //var builders = localBuilders
                //    .Where(t => t.GetInterfaces().Any())
                //    .ToList();

                //BuilderTypes.UnionWith(builders);

                //if (BaseAssembly != null)
                //    builders = baseBuilders
                //        .Where(t => t.GetInterfaces().Any())
                //        .ToList();

                BuilderTypes.UnionWith(iBuilders);
            }
        }

        #endregion
    }
}