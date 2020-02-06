using Devices.Core.Interfaces.Items;

namespace Devices.Core.Items.ItemGroups
{
    public interface ISiteInformationItems : IItemGroup
    {
        #region Public Properties
        CompositionType CompositionType { get; set; }
        string FirmwareVersion { get; set; }
        CorrectionFactorType PressureFactorLive { get; set; }
        string SerialNumber { get; set; }
        string SiteId1 { get; set; }
        string SiteId2 { get; set; }

        CorrectionFactorType SuperFactorLive { get; set; }
        CorrectionFactorType TemperatureFactorLive { get; set; }

        #endregion
    }

}