using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items.Attributes;

namespace Devices.Core.Items.ItemGroups
{
    public abstract class ItemGroup : IItemGroup
    {
  
        #region Protected

        public abstract void SetValues(IEnumerable<ItemValue> values);


        #endregion

        #region Private



        #endregion
    }
}