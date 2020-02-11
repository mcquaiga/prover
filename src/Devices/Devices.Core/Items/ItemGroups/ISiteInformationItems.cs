using Devices.Core.Interfaces;

namespace Devices.Core.Items.ItemGroups
{
    public interface ISiteInformationItems : IItemGroup
    {
        #region Public Properties
        CompositionType CompositionType { get; }
        string FirmwareVersion { get; set; }
        CorrectionFactorType PressureFactor { get; set; }
        string SerialNumber { get; set; }
        string SiteId1 { get; set; }
        string SiteId2 { get; set; }

        CorrectionFactorType SuperFactor { get; set; }
        CorrectionFactorType TemperatureFactor { get; set; }

        #endregion
    }

}