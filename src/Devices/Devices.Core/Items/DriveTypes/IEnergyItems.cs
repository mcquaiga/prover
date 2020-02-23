using Devices.Core.Items.ItemGroups;
using Shared;

namespace Devices.Core.Items.DriveTypes
{
    public class EnergyItems : ItemGroup
    {
        public virtual decimal EnergyGasValue { get; set; }
        public virtual decimal EnergyReading { get; set; }
        public virtual EnergyUnitType EnergyUnitType { get; set; }
    }
}