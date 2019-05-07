namespace Devices.Core.Interfaces.Items
{
    public interface IVolumeItems : IItemsGroup
    {
        decimal CorrectedMultiplier { get; }

        decimal CorrectedReading { get; }

        string CorrectedUnits { get; }

        decimal DriveRate { get; }

        string DriveRateDescription { get; }

        IDriveType DriveType { get; }

        decimal UncorrectedMultiplier { get; }

        decimal UncorrectedReading { get; }

        string UncorrectedUnits { get; }
    }
}