using Devices.Core;
using Devices.Core.Items.Attributes;
using Devices.Core.Items.ItemGroups;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    public class SiteInformationItems : HoneywellItemGroup, ISiteInformationItems
    {
        public CompositionType CompositionType { get; set; }

        [ItemInfo(122)]
        public string FirmwareVersion { get; set; }

        [ItemInfo(109)]
        public CorrectionFactorType PressureFactorLive { get; set; }

        [ItemInfo(62)]
        public string SerialNumber { get; set; }

        [ItemInfo(200)]
        public string SiteId1 { get; set; }

        [ItemInfo(201)]
        public string SiteId2 { get; set; }

        [ItemInfo(110)]
        public CorrectionFactorType SuperFactorLive { get; set; }

        [ItemInfo(111)]
        public CorrectionFactorType TemperatureFactorLive { get; set; }

    }

    /*
     *
     *     {
            get
            {
                if (PressureFactorLive == CorrectionFactorType.Live &&
                    TemperatureFactorLive == CorrectionFactorType.Live &&
                    SuperFactorLive == CorrectionFactorType.Live)
                    return CompositionType.PTZ;

                if (PressureFactorLive == CorrectionFactorType.Live)
                    return CompositionType.P;

                if (TemperatureFactorLive == CorrectionFactorType.Live)
                    return CompositionType.T;

                return CompositionType.Fixed;
            }
        }
     *
     */
}