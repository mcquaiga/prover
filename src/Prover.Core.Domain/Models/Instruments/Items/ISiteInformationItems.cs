namespace Prover.Domain.Models.Instruments.Items
{
    public interface ISiteInformationItems
    {
        string SerialNumber { get; }
        string SiteId1 { get; }
        string SiteId2 { get; }
        string FirmwareVersion { get; }
    }
}