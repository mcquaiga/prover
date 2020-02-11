using Devices.Core.Interfaces;

namespace Devices.Core.Items.ItemGroups
{
    public interface ISuperFactorItems : IItemGroup, IHaveFactor
    {
        decimal Co2 { get; }
        decimal N2 { get; }
        decimal SpecGr { get; }
    }
}