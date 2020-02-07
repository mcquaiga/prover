using Devices.Core.Interfaces.Items;

namespace Devices.Core.Items.ItemGroups
{
    public interface ISuperFactorItems : IItemGroup
    {
        decimal Co2 { get; }
        decimal N2 { get; }
        decimal SpecGr { get; }
    }
}