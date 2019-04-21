namespace Devices.Core.Interfaces.Items
{
    public interface ISiteInformationItems : IItemsGroup
    {
        #region Public Properties

        string FirmwareVersion { get; }
        string SerialNumber { get; }
        string SiteId1 { get; }
        string SiteId2 { get; }

        #endregion Public Properties
    }
}