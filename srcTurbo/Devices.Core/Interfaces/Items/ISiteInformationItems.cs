namespace Devices.Core.Interfaces.Items
{
    public interface ISiteInformationItems : IItemsGroup
    {
        string FirmwareVersion { get; }
        CorrectionFactor PressureFactor { get; }
        string SerialNumber { get; }
        string SiteId1 { get; }
        string SiteId2 { get; }

        CorrectionFactor SuperFactor { get; }
        CorrectionFactor TemperatureFactor { get; }
    }
}