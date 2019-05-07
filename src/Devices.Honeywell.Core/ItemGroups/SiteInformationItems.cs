using Devices.Core;
using Devices.Core.Interfaces.Items;
using Devices.Honeywell.Core.Attributes;

namespace Devices.Honeywell.Core.ItemGroups
{
    internal class SiteInformationItems : ItemGroupBase, ISiteInformationItems
    {
        [ItemInfo(122)]
        public string FirmwareVersion { get; protected set; }

        [ItemInfo(109)]
        public CorrectionFactor PressureFactor { get; protected set; }

        [ItemInfo(62)]
        public string SerialNumber { get; protected set; }

        [ItemInfo(200)]
        public string SiteId1 { get; protected set; }

        [ItemInfo(201)]
        public string SiteId2 { get; protected set; }

        [ItemInfo(110)]
        public CorrectionFactor SuperFactor { get; protected set; }

        [ItemInfo(111)]
        public CorrectionFactor TemperatureFactor { get; protected set; }
    }
}