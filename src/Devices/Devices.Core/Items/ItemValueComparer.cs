using System.Collections.Generic;

namespace Devices.Core.Items
{
    public class ItemValueComparer : IEqualityComparer<ItemValue>
    {
        public bool Equals(ItemValue x, ItemValue y)
        {
            if (x.Id == y.Id)
                return true;

            return false;
        }

        public int GetHashCode(ItemValue obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}