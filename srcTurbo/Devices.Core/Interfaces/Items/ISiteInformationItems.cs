namespace Devices.Core.Interfaces.Items
{
    public interface ISiteInformationItems : IItemsGroup
    {
        string FirmwareVersion { get; }
        CorrectionFactorType PressureFactor { get; }
        string SerialNumber { get; }
        string SiteId1 { get; }
        string SiteId2 { get; }

        CorrectionFactorType SuperFactor { get; }
        CorrectionFactorType TemperatureFactor { get; }
    }
}