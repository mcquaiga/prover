using Shared;

namespace Devices.Core.Items.ItemGroups
{
    public class SiteInformationItems : ItemGroup
    {
        public virtual CompositionType CompositionType { get; set; }
        public virtual string FirmwareVersion { get; set; }
        public virtual CorrectionFactorType PressureFactor { get; set; }
        public virtual string SerialNumber { get; set; }
        public virtual string SiteId1 { get; set; }
        public virtual string SiteId2 { get; set; }
        public virtual CorrectionFactorType SuperFactor { get; set; }
        public virtual CorrectionFactorType TemperatureFactor { get; set; }
    }
}