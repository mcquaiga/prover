namespace Devices.Core.Interfaces.Items
{
    public interface ISuperFactorItems : IItemsGroup
    {
        decimal Co2 { get; }
        decimal N2 { get; }
        decimal SpecGr { get; }
    }
}