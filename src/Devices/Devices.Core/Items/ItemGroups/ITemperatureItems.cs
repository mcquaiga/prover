using System.Reflection;

namespace Devices.Core.Items.ItemGroups
{
    public class TemperatureItems : ItemGroup, IHaveFactor
    {
        public virtual decimal Factor { get; set; }
        public virtual decimal Base { get; set; }
        public virtual decimal GasTemperature { get; set; }
        public virtual TemperatureUnitType Units { get; set; }
    }
}