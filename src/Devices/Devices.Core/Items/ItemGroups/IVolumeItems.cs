using Devices.Core.Interfaces;

namespace Devices.Core.Items.ItemGroups
{
    public interface IVolumeCorrectedItems : IItemGroup
    {
        decimal CorrectedMultiplier { get; }
        decimal CorrectedReading { get; }
        string CorrectedUnits { get; }
    }

    public interface IVolumeUncorrectedItems : IItemGroup
    {
        decimal UncorrectedMultiplier { get; }
        decimal UncorrectedReading { get; }
        string UncorrectedUnits { get; }
    }

    public interface IVolumeItems : IItemGroup, IVolumeCorrectedItems, IVolumeUncorrectedItems
    {
        VolumeInputType VolumeInputType { get; }

        decimal DriveRate { get; }

        string DriveRateDescription { get; }
    }
}