using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;
using Devices.Honeywell.Core.Items;
using Devices.Honeywell.Core.Items.ItemGroups;

namespace Devices.Honeywell.Core
{
    public class HoneywellDeviceInstance : DeviceInstance
    {
        public HoneywellDeviceInstance(DeviceType deviceType)
            : base(deviceType)
        {
        }

        #region Public Methods

        public override T GetItemsByGroup<T>()
        {
            return (T) GetItemsByGroup<T>(ItemValues);
        }

        public override T GetItemsByGroup<T>(IEnumerable<ItemValue> values)
        {
            return (T) (IItemGroup) GetItemGroup(values, typeof(T));
        }

        public sealed override void SetItemGroups(IEnumerable<ItemValue> itemValues)
        {
            ItemValues.Clear();
            ItemValues.UnionWith(itemValues);

            //LookupClasses();
            //LookupProperties();
        }

        private void LookupClasses()
        {
            var ts = Assembly.GetCallingAssembly().GetTypes();

            var types = ts.Where(t => t.GetInterfaces().Contains(typeof(IItemGroup)) && !t.IsAbstract);
            
            types.ToList().ForEach(x =>
            {
                var group = GetItemGroup(ItemValues, x);
                this.AddAttribute(group);
            });
        }

        private void LookupProperties()
        {
            var props = GetType()
                .GetProperties()
                .Where(p => p.PropertyType.GetInterfaces().Contains(typeof(IItemGroup)));

            foreach (var p in props)
            {
                var group = GetItemGroup(ItemValues, p.PropertyType);
                p.SetValue(this, @group);
            }
        }

        public override string ToString()
        {
            return $"Device Type: {DeviceType.Name}";
        }

        #endregion

        private static ItemGroup GetItemGroup(IEnumerable<ItemValue> values, Type groupType)
        {
            //if (!groupType.GetInterfaces().Contains(typeof(IItemGroup)))
            //    throw new Exception($"Type {groupType.Name} must inherit from {typeof(ItemGroup).Name}.");

            var itemType = groupType.GetMatchingItemGroupClass();
            if (itemType == null)
                throw new Exception($"Type {groupType.Name} could not be found.");

            var itemGroup = (ItemGroup) Activator.CreateInstance(itemType);

            if (values == null) values = new List<ItemValue>();

            itemGroup.SetValues(values);

            return itemGroup;
        }
    }
    
}