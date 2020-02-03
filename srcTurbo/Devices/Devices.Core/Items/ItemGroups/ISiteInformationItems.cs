using Devices.Core.Interfaces.Items;

namespace Devices.Core.Items.ItemGroups
{
    public interface ISiteInformationItems : IItemGroup
    {
        #region Public Properties

        string FirmwareVersion { get; }
        CorrectionFactorType PressureFactor { get; }
        string SerialNumber { get; }
        string SiteId1 { get; }
        string SiteId2 { get; }

        CorrectionFactorType SuperFactor { get; }
        CorrectionFactorType TemperatureFactor { get; }

        #endregion
    }
}