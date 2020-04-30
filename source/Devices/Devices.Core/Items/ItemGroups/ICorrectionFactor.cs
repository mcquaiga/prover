namespace Devices.Core.Items.ItemGroups
{
    public interface ICorrectionFactor : IItemGroup
    {
        decimal Factor { get; set; }
    }
}