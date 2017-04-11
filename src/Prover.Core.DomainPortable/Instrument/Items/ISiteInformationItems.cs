namespace Prover.Core.DomainPortable.Instrument.Items
{
    public interface ISiteInformationItems
    {
        string FirmwareVersion { get; }
        string SerialNumber { get; }
        string SiteId1 { get; }
        string SiteId2 { get; }
    }
}