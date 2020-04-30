using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devices.Core.Interfaces;
using Devices.Core.Items.Attributes;

namespace Devices.Core.Items.ItemGroups.Builders
{
    public abstract class ItemGroupBuilderBase<TGroup> where TGroup : ItemGroup
    {
        
        protected ItemGroupBuilderBase()
        {
          
        }

        #region Public Methods

        public virtual TGroup GetItemGroupInstance(Type type, IEnumerable<ItemValue> itemValues, DeviceType deviceType)
        {
            var itemGroup = GetItemGroupInstance(type, deviceType);

            return (TGroup) itemGroup.SetValues(deviceType, itemValues);
        }

        #endregion

        #region Protected

        protected virtual Assembly BaseAssembly { get; }

        protected virtual ItemGroup GetItemGroupInstance(Type groupType, DeviceType deviceType)
        {
            var itemType = groupType.GetMatchingItemGroupClass(deviceType);
            if (itemType == null)
                throw new Exception($"Type {groupType.Name} could not be found.");

            var itemGroup = (ItemGroup) Activator.CreateInstance(itemType);

            return itemGroup;
        }

        protected virtual IEnumerable<PropertyInfo> ItemInfoAttributes(Type groupType)
        {
            return groupType.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(Attributes.ItemInfoAttribute)) != null);
        }

        protected IEnumerable<Tuple<PropertyInfo, ItemValue>> MatchItemValuesWithPropertyInfo(Type groupType,
            IEnumerable<ItemValue> values)
        {
            return from value in values
                join prop in ItemInfoAttributes(groupType) on value.Id equals prop.GetNumber()
                select new Tuple<PropertyInfo, ItemValue>(prop, value);
        }

        protected virtual TGroup SetValues(TGroup itemGroup, IEnumerable<ItemValue> itemValues)
        {
            MatchItemValuesWithPropertyInfo(itemGroup.GetType(), itemValues).ToList()
                .ForEach(pair =>
                {
                    var (prop, item) = pair;
                    itemGroup.SetPropertyValue(prop, item);
                    //SetPropertyValue(itemGroup, prop, item);
                });

            return itemGroup;
        }

        #endregion
    }
}