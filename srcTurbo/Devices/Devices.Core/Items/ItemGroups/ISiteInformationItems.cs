using Devices.Core.Interfaces.Items;

namespace Devices.Core.Items.ItemGroups
{
    public interface ISiteInformationItems : IItemGroup
    {
        #region Public Properties

        string FirmwareVersion { get; }
        CorrectionFactorType PressureFactorLive { get; }
        string SerialNumber { get; }
        string SiteId1 { get; }
        string SiteId2 { get; }

        CorrectionFactorType SuperFactorLive { get; }
        CorrectionFactorType TemperatureFactorLive { get; }

        #endregion
    }
}