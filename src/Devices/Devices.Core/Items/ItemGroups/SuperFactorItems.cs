using System.Reflection;
using Devices.Core.Interfaces;

namespace Devices.Core.Items.ItemGroups
{
    //public abstract class SuperFactorItems : ItemGroup, IHaveFactor
    //{
    //    public abstract decimal Co2 { get; }
    //    public abstract decimal N2 { get; }
    //    public abstract decimal SpecGr { get; }
    //    public abstract decimal Factor { get; set; }
    //    public abstract void SetPropertyValue(PropertyInfo property, ItemValue value);
    //}

    public class SuperFactorItems : ItemGroup, IHaveFactor
    {
        public virtual  decimal Factor { get; set; }
        public virtual  decimal Co2 { get; set; }
        public virtual decimal N2 { get; set; }
        public virtual decimal SpecGr { get; set; }
    }
}