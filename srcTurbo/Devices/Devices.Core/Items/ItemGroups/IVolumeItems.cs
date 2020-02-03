using Devices.Core.Interfaces.Items;
using Devices.Core.Items.DriveTypes;

namespace Devices.Core.Items.ItemGroups
{
    public interface IVolumeItems : IItemGroup
    {
        decimal CorrectedMultiplier { get; }

        decimal CorrectedReading { get; }

        string CorrectedUnits { get; }

        decimal DriveRate { get; }

        string DriveRateDescription { get; }

        decimal UncorrectedMultiplier { get; }

        decimal UncorrectedReading { get; }

        string UncorrectedUnits { get; }
    }
}