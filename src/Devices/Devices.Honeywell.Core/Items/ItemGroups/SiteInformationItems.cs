using Devices.Core;
using Devices.Core.Items.Attributes;
using Devices.Core.Items.ItemGroups;

namespace Devices.Honeywell.Core.Items.ItemGroups
{
    public class SiteInformationItemsHoneywell : SiteInformationItems
    {
        public override CompositionType CompositionType
        {
            get
            {
                if (PressureFactor == CorrectionFactorType.Live && TemperatureFactor == CorrectionFactorType.Live)
                    return CompositionType.PTZ;

                if (PressureFactor == CorrectionFactorType.Live)
                    return CompositionType.P;

                if (TemperatureFactor == CorrectionFactorType.Live)
                    return CompositionType.T;

                return CompositionType.Fixed;
            }
        }

        [ItemInfo(122)]
        public override string FirmwareVersion { get; set; }

        [ItemInfo(109)]
        public override CorrectionFactorType PressureFactor { get; set; }

        [ItemInfo(62)]
        public override string SerialNumber { get; set; }

        [ItemInfo(200)]
        public override string SiteId1 { get; set; }

        [ItemInfo(201)]
        public override string SiteId2 { get; set; }

        [ItemInfo(110)]
        public override CorrectionFactorType SuperFactor { get; set; }

        [ItemInfo(111)]
        public override CorrectionFactorType TemperatureFactor { get; set; }

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