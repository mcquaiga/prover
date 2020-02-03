using Devices.Core;
using Devices.Core.Items.Attributes;
using Devices.Core.Items.ItemGroups;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    internal class SiteInformationItems : HoneywellItemGroup, ISiteInformationItems
    {
        [ItemInfo(122)]
        public string FirmwareVersion { get; protected set; }

        [ItemInfo(109)]
        public CorrectionFactorType PressureFactorLive { get; protected set; }

        [ItemInfo(62)]
        public string SerialNumber { get; protected set; }

        [ItemInfo(200)]
        public string SiteId1 { get; protected set; }

        [ItemInfo(201)]
        public string SiteId2 { get; protected set; }

        [ItemInfo(110)]
        public CorrectionFactorType SuperFactorLive { get; protected set; }

        [ItemInfo(111)]
        public CorrectionFactorType TemperatureFactorLive { get; protected set; }
    }
}