namespace Prover.InstrumentProtocol.Core.Models.Instrument.Items
{
    public interface ISiteInformationItems : IItemsGroup
    {
        string FirmwareVersion { get; }
        string SerialNumber { get; }
        string SiteId1 { get; }
        string SiteId2 { get; }
    }
}