using System.Collections.Generic;
using Devices.Core.Interfaces;
using Devices.Core.Items;

namespace Devices.Honeywell.Core
{
    public class HoneywellDeviceInstance : DeviceInstance
    {
        public HoneywellDeviceInstance(DeviceType deviceType)
            : base(deviceType)
        {
        }

        #region Public Methods
        protected override void SetValues(IEnumerable<ItemValue> itemValues)
        {
            ItemValues.Clear();
            ItemValues.UnionWith(itemValues);
        }

    
        public override string ToString()
        {
            return $"Device Type: {DeviceType.Name}";
        }

        #endregion
    }
    
}

//private void LookupClasses()
//{
//    var ts = Assembly.GetCallingAssembly().GetTypes();

//    var types = ts.Where(t => t.GetInterfaces().Contains(typeof(ItemGroup)) && !t.IsAbstract);
            
//    types.ToList().ForEach(x =>
//    {
//        var group = GetItemGroup(ItemValues, x);
//        this.AddAttribute(group);
//    });
//}

//private void LookupProperties()
//{
//    var props = GetType()
//        .GetProperties()
//        .Where(p => p.PropertyType.GetInterfaces().Contains(typeof(ItemGroup)));

//    foreach (var p in props)
//    {
//        var group = GetItemGroup(ItemValues, p.PropertyType);
//        p.SetValue(this, @group);
//    }
//}